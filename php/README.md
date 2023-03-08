# PHP

Console app using PHP demonstrating creating a shipment and getting a shipping label with the Urb-it Delivery API.

## Requirements
- PHP 8.2 or higher
- Optionally Docker

## Prerequisites
1. Copy `.env.template` to `.env`
2. Update values in `.env` (you will get them from Urb-it)

## Running the app on your machine
1. Install dependencies `php composer.phar install`
2. Start with `cd src && php index.php`

## Running the app with Docker
1. `docker build --target runner -t urb-it-php-demo .`
2. `docker run -v $(pwd)/output:/app/output -it --rm --name running-urb-it-php-demo urb-it-php-demo`
   - Feel free to change the output folder to your liking

## Sample output
```
Create delivery 📦
>> Done!
>> shipment_number: 196326671825
>> tracking_number: 196326671825-01

Get shipping label 🏷️
>> Done! 🌟
>> ZPL saved in 'output' folder

Bye bye 👋
```
