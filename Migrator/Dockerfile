FROM python:3.8-alpine
WORKDIR /app
COPY ./Migrator/ .
RUN python3 -m pip install -r requirements.txt

# Set the following env vars on run
ENV CREATOR_GUID='...'
ENV CONNECTIONSTRING_MYSQL='host=...;user=...;password=...;database=...'
ENV CONNECTIONSTRING_PSQL='host=...;user=...;password=...;database=...'

ENTRYPOINT ['python3', './src/main.py']