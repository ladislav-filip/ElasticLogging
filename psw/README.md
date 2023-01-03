# Autentifikace
Soubor s nastavením přístupů přes Basic HTTP autentifikaci.

Pro zapnutí autentifikae je potřeba vytvořit **.htpasswd** soubor obsahující dvojice login a heslo.

Nejprve nainstalujemeutilitu pro vytvoření souboru s loginem a heslem:
```bash
sudo apt-get update
sudo apt-get install apache2-utils
```

Následně vygenerujeme přístupové účty úpomocí příkazu:
```bash
sudo htpasswd -c /etc/nginx/.htpasswd pil
```
kde "pil" je uživatelské jmeno/login.

Aktuláně přiložený .htpasswd soubor obsahuje přihlašovací údaje
```
Login: pil
Heslo: tukan
```

Tento vytvořený soubor se pak připojí v "nginx" pomocí konfigurace:
```json
        auth_basic              "Restricted Content";
        auth_basic_user_file    /psw/.htpasswd;
```