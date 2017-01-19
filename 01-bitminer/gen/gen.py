from flask import Flask, Response
import os
import socket
import time

app = Flask(__name__)

# Enable debugging if the DEBUG environment variable is set and starts with Y
app.debug = os.environ.get("DEBUG", "").lower().startswith('y')

urandom = os.open("/dev/urandom", os.O_RDONLY)


@app.route("/")
def index():
    return "gen running"


@app.route("/<int:how_many_bytes>")
def gen(how_many_bytes):
    return Response(
        os.read(urandom, how_many_bytes),
        content_type="application/octet-stream")


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
