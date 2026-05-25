#!/bin/sh
# Runs as part of the nginx docker-entrypoint.d pipeline before nginx starts.
# Generates a temporary self-signed certificate so nginx can boot while
# acme.sh issues the real Let's Encrypt certificate in the background.

CERT_DIR="/etc/nginx/certs"

if [ ! -f "${CERT_DIR}/fullchain.pem" ]; then
    echo "10-generate-self-signed: no certificate found — generating temporary self-signed certificate"
    mkdir -p "${CERT_DIR}"
    openssl req -x509 -nodes -newkey rsa:4096 -days 1 \
        -keyout "${CERT_DIR}/privkey.pem" \
        -out    "${CERT_DIR}/fullchain.pem" \
        -subj   "/CN=staging.naeraces.co.uk"
fi
