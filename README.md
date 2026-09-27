# HTML Scraping API

REST API для обработки HTML-страниц и извлечения данных по CSS-селектору.

Сервис принимает URL и HTML-код страницы в формате Base64, находит элементы по указанному CSS-селектору, извлекает значения заданного HTML-атрибута, сохраняет найденные элементы в PostgreSQL и извлекает email-адреса из содержимого страницы.

Дополнительно сервис выполняет расшифровку переданного текста с использованием AES-256 в режиме ECB.

## Основные возможности

* Валидация входных данных с помощью FluentValidation.
* Декодирование URL и HTML-кода из Base64.
* Парсинг HTML в DOM с помощью AngleSharp.
* Поиск элементов по CSS-селектору.
* Извлечение значений HTML-атрибутов.
* Сохранение найденных элементов в PostgreSQL.
* Извлечение email-адресов из HTML.
* Расшифровка данных с использованием AES-256 ECB.
* Swagger UI для тестирования API.

## Требования

Для запуска проекта необходимы:

* Docker
* Docker Compose

Запуск приложения выполняется через Docker Compose.

## Запуск

### 1. Клонирование репозитория

```bash
git clone https://github.com/TheCunningRedFox/HTML-Scraping-API.git
cd HTML-Scraping-API
```

### 2. Конфигурация

Для запуска используется файл `.env`, расположенный в корне проекта.

Пример:

```env
POSTGRES_DB=postgres
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres_password

POSTGRES_PORT=5432
PGADMIN_PORT=8080

PGADMIN_EMAIL=admin@example.com
PGADMIN_PASSWORD=admin_password
```

### Переменные окружения

#### PostgreSQL

| Переменная          | Описание                       |
| ------------------- | ------------------------------ |
| `POSTGRES_DB`       | Имя базы данных                |
| `POSTGRES_USER`     | Пользователь PostgreSQL        |
| `POSTGRES_PASSWORD` | Пароль пользователя PostgreSQL |
| `POSTGRES_PORT`     | Порт PostgreSQL                |

#### pgAdmin

| Переменная         | Описание                    |
| ------------------ | --------------------------- |
| `PGADMIN_PORT`     | Порт веб-интерфейса pgAdmin |
| `PGADMIN_EMAIL`    | Email пользователя pgAdmin  |
| `PGADMIN_PASSWORD` | Пароль пользователя pgAdmin |

### 3. Запуск

Собрать и запустить контейнеры:

```bash
docker compose up --build
```

Для запуска в фоновом режиме:

```bash
docker compose up --build -d
```

### 4. Остановка

```bash
docker compose down
```

### 5. Просмотр логов

```bash
docker compose logs -f
```

## Swagger

После запуска приложения Swagger UI доступен по адресу:

```text
http://localhost:8090/api/swagger
```

Swagger позволяет ознакомиться с API и выполнить запросы непосредственно из браузера.

---

# REST API

## POST `api/elements`

Метод выполняет обработку HTML-страницы и возвращает результаты анализа.

В рамках одного запроса сервис:

1. Проверяет входные параметры.
2. Декодирует URL и HTML-код страницы из Base64.
3. Находит элементы по CSS-селектору.
4. Извлекает значение указанного атрибута каждого найденного элемента.
5. Сохраняет найденные элементы в PostgreSQL.
6. Извлекает email-адреса из HTML-кода страницы.
7. Расшифровывает переданный зашифрованный текст.
8. Возвращает результаты обработки.

### URL

```http
POST /api/elements
```

### Заголовки

```http
Content-Type: application/json
```

### Тело запроса

```json
{
  "selector": "script[src]",
  "attribute": "src",
  "url_b64": "[BASE64_URL]",
  "page_b64": "[BASE64_HTML]",
  "encrypted_text_bytes_b64": "[BASE64_ENCRYPTED_TEXT]",
  "key_bytes_b64": "[BASE64_KEY]"
}
```

### Параметры

| Параметр                   | Тип      | Обязательный | Описание                                                               |
| -------------------------- | -------- | ------------ | ---------------------------------------------------------------------- |
| `selector`                 | `string` | Да           | CSS-селектор для поиска элементов.                                     |
| `attribute`                | `string` | Да           | HTML-атрибут, значение которого необходимо получить. Например, `href`. |
| `url_b64`                  | `string` | Да           | URL в формате Base64.                                                  |
| `page_b64`                 | `string` | Да           | HTML-код страницы в формате Base64.                                    |
| `encrypted_text_bytes_b64` | `string` | Да           | Зашифрованный текст в формате Base64.                                  |
| `key_bytes_b64`            | `string` | Да           | Ключ шифрования в формате Base64.                                      |

### Пример запроса

```bash
curl -X POST "http://localhost:8090/api/elements" \
  -H "Content-Type: application/json" \
  -d '{
    "selector": "script[src]",
    "attribute": "src",
    "url_b64": "[BASE64_URL]",
    "page_b64": "[BASE64_HTML]",
    "encrypted_text_bytes_b64": "[BASE64_ENCRYPTED_TEXT]",
    "key_bytes_b64": "[BASE64_KEY]"
  }'
```

## Ответ

При успешной обработке API возвращает HTTP `200 OK`.

```json
{
  "is_error": 0,
  "error_code": "",
  "error_message": "",
  "elements_count": 2,
  "emails_count": 1,
  "url": "https://example.com",
  "decrypted_plain_text": "Decrypted text",
  "elements_attr_list": [
    "https://example.com/page1",
    "https://example.com/page2"
  ],
  "emails_list": [
    "user@example.com"
  ]
}
```

### Поля ответа

| Поле                   | Тип            | Описание                                          |
| ---------------------- | -------------- | ------------------------------------------------- |
| `is_error`             | `int`          | `0` — успешная обработка, `1` — ошибка.           |
| `error_code`           | `string`       | Код ошибки. При успешной обработке пустой.        |
| `error_message`        | `string`       | Описание ошибки. При успешной обработке пустой.   |
| `elements_count`       | `int`          | Количество найденных элементов.                   |
| `emails_count`         | `int`          | Количество найденных email-адресов.               |
| `url`                  | `string`       | URL после декодирования Base64.                   |
| `decrypted_plain_text` | `string`       | Расшифрованный текст.                             |
| `elements_attr_list`   | `List<string>` | Значения указанного атрибута найденных элементов. |
| `emails_list`          | `List<string>` | Найденные email-адреса.                           |

## Ошибки

При ошибке обработки сервис возвращает HTTP `400 Bad Request` или `500 Internal Server Error`.

Пример:

```json
{
  "is_error": 1,
  "error_code": "BASE64_DECODE_ERROR",
  "error_message": "Ошибка при декодировании Base64 в Page."
}
```

### Коды ошибок

| HTTP  | Код                     | Описание                                          |
| ----- | ----------------------- | ------------------------------------------------- |
| `400` | `VALIDATION_ERROR`      | Некорректные или отсутствующие входные параметры. |
| `400` | `BASE64_DECODE_ERROR`   | Ошибка декодирования значения из Base64.          |
| `400` | `DECRYPTION_ERROR`      | Ошибка при расшифровке данных.                    |
| `500` | `INTERNAL_SERVER_ERROR` | Непредвиденная ошибка сервера.                    |

## Шифрование

Для расшифровки используется:

```text
AES-256
Mode: ECB
Padding: None
```

Ключ передаётся в параметре `key_bytes_b64` в формате Base64 и после декодирования должен иметь длину **32 байта**.

Зашифрованные данные передаются в параметре `encrypted_text_bytes_b64`.

## Хранение данных

Найденные HTML-элементы сохраняются в PostgreSQL в таблицу `elements`.

Для работы с базой данных используется Dapper.

В таблице сохраняются:

* идентификатор элемента;
* значение найденного HTML-атрибута;
* полный HTML-код элемента.

## Используемые технологии

* **.NET / ASP.NET Core** — REST API
* **FluentValidation** — валидация входных данных
* **AngleSharp** — анализ и парсинг HTML
* **Dapper** — работа с PostgreSQL
* **PostgreSQL** — хранение найденных элементов
* **Docker / Docker Compose** — запуск приложения и инфраструктуры
* **Swagger / OpenAPI** — документация API
