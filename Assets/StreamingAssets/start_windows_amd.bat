@echo off
cd /d "%~dp0"

IF NOT EXIST "python310\python.exe" (
    echo [First Time Setup] Downloading Portable Python 3.10...
    curl -L -o python_pkg.zip https://www.nuget.org/api/v2/package/python/3.10.11
    
    echo [First Time Setup] Extracting Python...
    tar -xf python_pkg.zip tools
    move tools python310
    del python_pkg.zip
    
    echo [First Time Setup] Installing AMD/Intel dependencies...
    python310\python.exe -m pip install -r requirements-amd.txt
)

echo Starting AI Backend...
python310\python.exe PixelImageTransformer_withUnityTCP.py