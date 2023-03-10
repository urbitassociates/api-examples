# PHP

Console app using PHP demonstrating creating a shipment and getting a shipping label with the Urb-it Delivery API.

## Requirements
- PHP 8.1 or higher

## Prerequisites
1. Copy `.env.template` to `.env`
2. Update values in `.env` (you will get them from Urb-it)

## Running the app on your machine
1. Install dependencies `php composer.phar install`
2. Start with `cd src && php index.php`

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
