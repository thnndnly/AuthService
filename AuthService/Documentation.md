# Authentication & User Management API

## 📌 **Einleitung**
Diese API ermöglicht die **Benutzerverwaltung und Authentifizierung** mittels **JWT (JSON Web Token) und Refresh Tokens**. Die Authentifizierung erfolgt über **Access Tokens**, die nach einer bestimmten Zeit ablaufen. Für die Erneuerung wird ein **Refresh Token** verwendet.

---

## 🔑 **JWT & Refresh Token Erklärung**
### **📌 Was ist ein JWT (Access Token)?**
Ein **JWT (JSON Web Token)** ist ein signierter Token, der eine bestimmte **Gültigkeit (`exp`)** hat und **Benutzerdaten** enthält.

#### **🔹 Beispiel JWT (dekodiert):**
auf der Webseite https://jwt.io/ kannst du gucken wie diesee JWT Tokens aufgebaut sind.
```json
{
  "unique_name": "userAdminOG",
  "role": "Admin",
  "nbf": 1739921848,
  "exp": 1739925448,
  "iat": 1739921848
}
```
- **`unique_name`** → Der Benutzername.
- **`role`** → Benutzerrolle (`Admin` oder `Nutzer`).
- **`nbf` (Not Before)** → Token ist erst nach diesem Zeitpunkt gültig.
- **`exp` (Expiration Time)** → Token läuft nach diesem Zeitpunkt ab.
- **`iat` (Issued At)** → Zeitpunkt, an dem das Token erstellt wurde.

### **📌 Was ist ein Refresh Token?**
Ein **Refresh Token** dient dazu, ein neues JWT zu generieren, wenn das alte **abgelaufen** ist. Es wird **an die ursprüngliche IP-Adresse gebunden**, um Missbrauch zu verhindern.

#### **🔹 Beispiel Refresh Token:**
```json
{
  "token": "c2VjdXJlIG5ldyByZWZyZXNoIHRva2Vu",
  "username": "userAdminOG",
  "expiration": "2025-02-25T12:00:00Z",
  "ipAddress": "192.168.1.100"
}
```
- **`token`** → Ein eindeutiger, zufällig generierter String.
- **`username`** → Der zugehörige Benutzername.
- **`expiration`** → Ablaufdatum des Refresh Tokens.
- **`ipAddress`** → Bindet das Token an die ursprüngliche IP-Adresse.

---

## 📌 **API-Endpunkte**

### **1️⃣ Registrierung eines neuen Nutzers**
🔹 **POST `/api/auth/register`**
#### **📌 Request:**
```json
{
  "username": "userAdminOG",
  "password": "1312ogadmin1312",
  "role": "Admin"
}
```
#### **✅ Response:**
```json
"User registered successfully."
```
#### **Response:**
````json
"Password is not valid."
````
Das Passwort muss folgende Bedingungen erfüllen.
1. Das Passwort muss mindestens 12 Zeichen lang sein.
2. Es muss mindestens einen Großbuchstaben enthalten (A-Z).
3. Es muss mindestens einen Kleinbuchstaben enthalten (a-z).
4. Es muss mindestens eine Ziffer enthalten (0-9).
5. Es muss mindestens ein Sonderzeichen enthalten, also ein Zeichen, das weder Buchstabe noch Ziffer ist (z. B. @, #, ?, ! etc.).
### **2️⃣ Benutzer-Login (JWT & Refresh Token erhalten)**
🔹 **POST `/api/auth/login`**
#### **📌 Request:**
```json
{
  "username": "userAdminOG",
  "password": "1312ogadmin1312"
}
```
#### **✅ Response:**
```json
{
  "accessToken":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImRpbWl0cmkiLCJyb2xlIjoiTnV0emVyIiwibmJmIjoxNzM5OTkwMDU2LCJleHAiOjE3Mzk5OTM2NTYsImlhdCI6MTczOTk5MDA1Nn0.4JAzBlj4zJthIg7lCwZ64PIDBTLgDiovntTVHro55Ao",
  "refreshToken":"S2KLOiEshLUQNj85W6oiUuRN3rOLcQgxZ6tMsTzMWfHozgRTH0kUDadKYAGXOByx4RZkU1v5XC6LmtAn/JzBNA=="
}
```
### **4️⃣ JWT erneuern mit Refresh Token (IP-Validierung)**
🔹 **POST `/api/auth/refresh`**
#### **📌 Request:**
```json
{
  "refreshToken": "S2KLOiEshLUQNj85W6oiUuRN3rOLcQgxZ6tMsTzMWfHozgRTH0kUDadKYAGXOByx4RZkU1v5XC6LmtAn/JzBNA=="
}
```
#### **✅ Response:**
```json
{
  "accessToken":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImRpbWl0cmkiLCJyb2xlIjoiTnV0emVyIiwibmJmIjoxNzM5OTkwMjgyLCJleHAiOjE3Mzk5OTM4ODIsImlhdCI6MTczOTk5MDI4Mn0.ULagpc2MSTwFgW3spF6Hb6x1d61ttOamUzNkIwf8q0A",
  "refreshToken":"djww3G36Rv6ZIYqj/in5CU7ymqpTSmFg55tzzQhxj42GyhDfn00R7rF82dEmJ/4zWKAh2AKbswyk7ibmIho0OA=="
}
```
❌ **Falls die IP-Adresse nicht übereinstimmt, wird das Token abgelehnt.**

### **5️⃣ Passwort ändern**
🔹 **POST `/api/auth/change-password`**
#### **📌 Request:**
```json
{
  "username": "userAdminOG",
  "oldPassword": "1312ogadmin1312",
  "newPassword": "newSecurePassword!"
}
```
#### **✅ Response:**
```json
"Password updated successfully."
```

### **6️⃣ Benutzer löschen**
🔹 **POST `/api/auth/delete-user`**
#### **📌 Request:**
```json
{
  "username": "userAdminOG",
  "password": "newSecurePassword!"
}
```
#### **✅ Response:**
```json
"User deleted successfully."
```

### **7️⃣ Nutzerverwaltung für Admins**
🔹 **GET `/api/admin/users`** → Liste aller Nutzer abrufen (Admin-only)
🔹 **POST `/api/admin/update-role`** → Rolle eines Nutzers ändern
🔹 **POST `/api/admin/delete-user`** → Nutzer löschen

---

## 🔐 **Sicherheitsmaßnahmen**
✔ **JWTs haben begrenzte Lebensdauer (`exp` im Token).**  
✔ **Refresh Tokens sind an die ursprüngliche IP-Adresse gebunden.**  
✔ **Passwörter werden mit BCrypt gehasht.**  
✔ **Admin-Routen sind mit `Authorize(Roles = "Admin")` geschützt.**  
✔ **Maximale Login-Versuche verhindern Brute-Force-Angriffe.**  
✔ **Rate-Limiting kann hinzugefügt werden, um API-Spamming zu verhindern.**

---

## 📌 **Fazit**
Dieses Dokument beschreibt die **komplette Authentifizierungs-API**, inklusive **JWT- & Refresh Tokens**, Admin-Verwaltung und Sicherheitsfeatures. Es stellt sicher, dass **Benutzer sicher authentifiziert** und **Zugriffsrechte korrekt verwaltet** werden. 🚀

