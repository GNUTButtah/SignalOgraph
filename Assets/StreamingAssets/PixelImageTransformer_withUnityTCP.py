import torch
from diffusers import StableDiffusionImg2ImgPipeline
from PIL import Image
import socket



# --- Settings ---
MODEL_ID = "runwayml/stable-diffusion-v1-5"
INPUT_IMAGE = "newPixelInput.png"

NEGATIVE_PROMPT = (
    "blurry, low resolution, blocky style, TWO suns"
)

# Prompts für jedes level
PROMPTS = {
    "1": "make it look like a much more detailed handmade painting of a fantasy purple swamp landscape, foggy, dark green tones, mystical atmosphere. keep the locations and proportions of the objects but keep their shape.  dark green = leaves, light brown = dry leaves, dark brown = tree trunk, deep purple = sky, light purple = clouds, yellow = MOON",
    "2": "make it look like a much more detailed handmade painting of a fantasy desert landscape. keep the locations and proportions of the objects but keep their shape. Everything orange = sand, light green = green sky, red = canyon mountains, white = clouds and dark green = cactus. the ONE yellow spot is ONE sun.",
    "3": "mountain landscape, snowy peaks, dramatic sky, high detail"
}

DEFAULT_PROMPT = "TURN THE ENTIRE SCEEN INTO AN ALL WHITE CANVAS"

# device für die Generierung auswählen (vorher ging es nur mit NVIDIA wegen cuda dependency),  update: nvidia ist bis jetzt noch immer die einzige auf Funktion getestete Option. Zumindest bei Aijub (Intel Arc iGPU) ging die directml pipeline immer noch nicht
device = "cpu"
dtype = torch.float32
generator_device = "cpu"

try:
    if torch.cuda.is_available():
        device = "cuda"
        dtype = torch.float16
        generator_device = "cuda"
        print("Using CUDA (NVIDIA GPU)")

    elif hasattr(torch.backends, "mps") and torch.backends.mps.is_available():
        device = "mps"
        dtype = torch.float16
        generator_device = "mps"
        print("Using MPS (Apple Silicon)")

    else:
        # DirectML nur wenn CUDA/MPS nicht verfügbar
        try:
            import torch_directml
            device = torch_directml.device()
            dtype = torch.float16
            generator_device = device
            print("Using DirectML (AMD/Intel GPU)")
        except ImportError:
            print("DirectML not available → falling back to CPU")

except Exception as e:
    print("Device detection error:", e)
    print("Falling back to CPU")

print("Final device:", device)

# --- TCP SETUP ---
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.bind(("localhost", 5007))
sock.listen(1)

print("Waiting for Unity...")
conn, addr = sock.accept()
print("Connected to Unity:", addr)


# Stable diffusion laden
pipe = StableDiffusionImg2ImgPipeline.from_pretrained(
    MODEL_ID,
    torch_dtype=dtype,
    safety_checker=None
).to(device)

if device == "cuda":
    pipe.enable_xformers_memory_efficient_attention()

print("Model loaded. Waiting for commands...")

# --- MAIN LOOP ---
while True:
    try:
        unityRecievedData = conn.recv(1024).decode("utf-8").strip()
    except:
        print("Connection lost, shutting down AI backend")
        break

    if not unityRecievedData:
        continue

    print("Received:", unityRecievedData)

    # 🟩 NEU: GENERATE mit Level (z.B. GENERATE|2)
    if unityRecievedData.startswith("GENERATE"):

        try:
            # unity message zerlegen (also nur die level ID rausnehmen)
            parts = unityRecievedData.split("|")
            level = parts[1] if len(parts) > 1 else "0"

            # prompt für das level holen
            prompt = PROMPTS.get(level, DEFAULT_PROMPT)

            print(f"Generating for level {level} with prompt: {prompt}")

            # Pixel input image laden und upscalen
            init_image = Image.open(INPUT_IMAGE).convert("RGB")
            init_image = init_image.resize((1536, 512), Image.NEAREST)

            # Generieren
            result = pipe(
                prompt=prompt,
                negative_prompt=NEGATIVE_PROMPT,
                image=init_image,
                strength=0.9,
                guidance_scale=9.5,
                num_inference_steps=30,
                generator=torch.Generator(generator_device)
            )

            # Level-spezifischer Output
            output_file = f"output_level_{level}.png"
            result.images[0].save(output_file)

            print(f"Landscape generated: {output_file}")

            # Antwort mit Level
            conn.sendall((f"DONE|{level}\n").encode("utf-8"))

        except Exception as e:
            print("Error during generation:", e)
            conn.sendall(("ERROR\n").encode("utf-8"))

print("Server offline.")
sock.close()