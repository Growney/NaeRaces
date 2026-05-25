FROM nginx:mainline-alpine
RUN apk add --no-cache openssl
COPY 10-generate-self-signed.sh /docker-entrypoint.d/10-generate-self-signed.sh
RUN chmod +x /docker-entrypoint.d/10-generate-self-signed.sh
