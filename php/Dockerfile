FROM composer as builder
WORKDIR /tmp/composer-deps
COPY composer.* ./
RUN composer install

FROM php:8.2-cli-alpine3.17 as runner
COPY --from=builder /tmp/composer-deps /app/vendor
COPY . /app/
WORKDIR /app/src
CMD [ "php", "./index.php" ]