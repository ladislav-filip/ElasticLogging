openssl req -newkey rsa:4096 \
            -x509 \
            -sha256 \
            -days 3650 \
            -nodes \
            -out elastic.crt \
            -keyout elastic.key \
            -subj "/C=CZ/ST=Czech/L=Ostrava/O=Security/OU=Dev/CN=elastic.local"

## Nastavení parametrů
# C= - Country name. The two-letter ISO abbreviation.
# ST= - State or Province name.
# L= - Locality Name. The name of the city where you are located.
# O= - The full name of your organization.
# OU= - Organizational Unit.
# CN= - The fully qualified domain name.

## Certifikát lze následně ověřit a prohlédnout pomocí:
# openssl s_client -connect localhost:443