@echo off
:: Navigate to the folder where this batch file is located
cd /d "%~dp0"

:: Check if our portable python folder exists
IF NOT EXIST "python310\python.exe" (
    echo [First Time Setup] Downloading Portable Python 3.10...
    :: Download the official Python 3.10 package
    curl -L -o python_pkg.zip https://www.nuget.org/api/v2/package/python/3.10.11
    
    echo [First Time Setup] Extracting Python...
    :: Extract only the 'tools' folder (which contains python and pip)
    tar -xf python_pkg.zip tools
    
    :: Rename 'tools' to 'python310' to be clean
    move tools python310
    
    :: Delete the zip file to save space
    del python_pkg.zip
    
    echo [First Time Setup] Installing NVIDIA PyTorch and dependencies...
    :: Use our specific portable python to run pip
    python310\python.exe -m pip install -r requirements-nvidia.txt
)

echo Starting AI Backend...
:: Run the script using the portable python
python310\python.exe PixelImageTransformer_withUnityTCP.py