#!/bin/sh
# Runs inside the neilpang/acme.sh container.
#
# First run:  registers the ACME account, issues the certificate via the
#             Namecheap DNS-01 challenge, and installs it to the shared
#             /certs volume, then signals nginx to reload.
# Subsequent: acme.sh daemon handles automatic renewal and re-runs the
#             reload command when a renewed cert is installed.
#
# Required environment variables (set via docker-compose .env):
#   ACME_EMAIL           – Let's Encrypt account e-mail
#   NAMECHEAP_USERNAME   – Namecheap account username
#   NAMECHEAP_API_KEY    – Namecheap API key
#   NAMECHEAP_SOURCEIP   – Server IP whitelisted in Namecheap API settings

set -e

DOMAIN="staging.naeraces.co.uk"

# curl against the Docker socket is used to send SIGHUP to the nginx
# container without needing the Docker CLI binary.
RELOAD_CMD="curl --unix-socket /var/run/docker.sock -sX POST 'http://localhost/containers/naeraces-nginx/kill?signal=HUP'"

# Register ACME account (safe to run repeatedly — no-op if already registered)
acme.sh --register-account -m "${ACME_EMAIL}"

# Issue and install the certificate on first run only.
# Renewal is handled automatically by the daemon below.
if [ ! -d "/acme.sh/${DOMAIN}" ]; then
    echo "acme-init: issuing certificate for ${DOMAIN} via Namecheap DNS-01 challenge"
    acme.sh --issue --dns dns_namecheap -d "${DOMAIN}"
    acme.sh --install-cert -d "${DOMAIN}" \
        --fullchain-file /certs/fullchain.pem \
        --key-file       /certs/privkey.pem \
        --reloadcmd      "${RELOAD_CMD}"
fi

# Hand off to the acme.sh renewal daemon (runs cron internally)
exec acme.sh daemon
