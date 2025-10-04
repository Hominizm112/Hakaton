import http.server
import os
import socketserver
import sys


class WebGLHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        # Handle Brotli files
        if self.path.endswith('.br'):
            self.send_header('Content-Encoding', 'br')
            # Set correct content types
            if '.js' in self.path:
                self.send_header('Content-Type', 'application/javascript')
            elif '.wasm' in self.path:
                self.send_header('Content-Type', 'application/wasm')
            elif '.data' in self.path:
                self.send_header('Content-Type', 'application/octet-stream')
        super().end_headers()
    
    def do_GET(self):
        # Serve index.html for root path
        if self.path == '/':
            self.path = '/index.html'
        
        # Check for Brotli compressed files
        file_path = self.translate_path(self.path)
        br_path = file_path + '.br'
        
        if os.path.exists(br_path):
            self.path = self.path + '.br'
        
        return super().do_GET()

PORT = 8000



# Change to build directory if needed
build_dir = 'Builds'
if os.path.exists(build_dir):
    os.chdir(build_dir)
    print(f"Changed to directory: {os.getcwd()}")
    
    print("Current directory:", os.getcwd())
# print("Files in directory:", os.listdir('.'))

print(f"Starting server on port {PORT}...")
print("Make sure you can access: http://localhost:8000")
print("Then run: ssh -R 80:localhost:8000 serveo.net")

try:
    with socketserver.TCPServer(("", PORT), WebGLHTTPRequestHandler) as httpd:
        print(f"✅ Server is running at http://localhost:{PORT}")
        print("Press Ctrl+C to stop")
        httpd.serve_forever()
except OSError as e:
    print(f"❌ Error: {e}")
    print("Port 8000 might be in use. Try a different port:")
    print("python serveo_server.py 8080")
except KeyboardInterrupt:
    print("\nServer stopped.")