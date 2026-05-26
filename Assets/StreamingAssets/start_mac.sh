#!/bin/bash
cd "$(dirname "$0")"

# Quick check to make sure they haven't uninstalled their Mac's default Python 3
if ! command -v python3 &> /dev/null; then
    echo "Error: Python 3 is not installed on this Mac. Please install it from python.org to play."
    exit 1
fi

if [ ! -d "unity_ml_env" ]; then
    echo "First time setup: Creating Python environment..."
    python3 -m venv unity_ml_env
    
    echo "Activating environment..."
    source unity_ml_env/bin/activate
    
    echo "Installing Mac PyTorch and dependencies..."
    # On Mac, pip sometimes needs an explicit upgrade first
    pip install --upgrade pip
    pip install -r requirements-mac.txt
else
    source unity_ml_env/bin/activate
fi

echo "Starting AI Backend..."
python PixelImageTransformer_withUnityTCP.py