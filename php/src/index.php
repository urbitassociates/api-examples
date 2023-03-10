<?php

require_once '../vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Exception\GuzzleException;
use GuzzleHttp\Exception\RequestException;

class Program
{
    private static Client $httpClient;
    private string $clientId;

    public static function main(): void
    {
        $program = new Program();
        $program->start();
    }

    public function __construct()
    {
        $dotenv = Dotenv\Dotenv::createImmutable(__DIR__ . '/..');
        $dotenv->load();
        $dotenv->required(['AUTHORIZATION', 'CLIENT_ID'])->notEmpty();

        // You'll get the authorization token from Urb-it
        $authorization = $_ENV["AUTHORIZATION"];

        $headers = [
            'Accept' => 'application/json',
            'Authorization' => $authorization,
        ];

        // You'll get the client id from Urb-it
        $this->clientId = $_ENV["CLIENT_ID"];

        self::$httpClient = new Client([
            'base_uri' => 'https://sandbox.urb-it.com/v4/',
            'headers' => $headers,
        ]);
    }

    public function start(): void
    {
        echo("Create delivery 📦" . PHP_EOL);
        try {
            $shipmentInformation = $this->createShipment();
            $shipmentNumber = $shipmentInformation[0];
            $trackingNumber = $shipmentInformation[1];
            echo(">> Done!" . PHP_EOL);
            echo(">> shipment_number: $shipmentNumber" . PHP_EOL);
            echo(">> tracking_number: $trackingNumber" . PHP_EOL);
            echo(PHP_EOL);
            echo("Get shipping label 🏷️" . PHP_EOL);
            $this->shippingLabel($trackingNumber);
            echo(">> Done! 🌟" . PHP_EOL);
            echo(">> ZPL saved in 'output' folder" . PHP_EOL);
            echo(PHP_EOL);
            echo("Bye bye 👋" . PHP_EOL);
        } catch(RequestException $e) {
            fwrite(STDERR, "Request failed" . PHP_EOL);
            fwrite(STDERR, "Request: " . GuzzleHttp\Psr7\Message::toString($e->getRequest()) . PHP_EOL);
            fwrite(STDERR, "Response: " . GuzzleHttp\Psr7\Message::toString($e->getResponse()) . PHP_EOL);
        } catch (GuzzleException $e) {
            fwrite(STDERR, "GuzzleException: " . $e->getMessage() . PHP_EOL);
        } catch (Exception $e) {
            fwrite(STDERR, "Exception: " . $e->getMessage() . PHP_EOL);
        }
    }

    /**
     * @throws Exception|GuzzleException
     */
    private function createShipment(): array
    {
        $data = $this->getShipmentData();

        $body = json_encode($data);
        $response = self::$httpClient->post('shipments', [
            'headers' => [
                'Content-Type' => 'application/json',
            ],
            'body' => $body,
        ]);

        if ($response->getStatusCode() !== 201) {
            throw new Exception("Error! Unexpected status code: " . $response->getStatusCode());
        }

        $json = json_decode($response->getBody(), true);
        $shipmentNumber = $json['shipment_number'];
        $deliveries = $json['deliveries'];
        $trackingNumber = $deliveries[0]['tracking_number'];

        return [$shipmentNumber, $trackingNumber];
    }

    /**
     * @throws Exception|GuzzleException
     */
    private function shippingLabel(string $trackingNumber): void
    {
        $path = __DIR__ . '/../output/' . $trackingNumber . ".zpl";
        self::$httpClient->get("deliveries/$trackingNumber/shipping-label", ["sink" => $path]);
    }

    private function getShipmentData(): array
    {
        return [
            'client_id' => $this->clientId,
            'service_type' => 'NEXT_DAY_DELIVERY',
            'reference_id' => [
                'description' => 'Order Id',
                'data' => 'SampleCode1337',
            ],
            'deliveries' => [
                [
                    'weight' => [
                        'unit' => 'g',
                        'value' => 550,
                    ],
                    'dimensions' => [
                        'height' => [
                            'unit' => 'cm',
                            'value' => 10,
                        ],
                        'width' => [
                            'unit' => 'cm',
                            'value' => 15,
                        ],
                        'length' => [
                            'unit' => 'cm',
                            'value' => 20,
                        ],
                    ],
                    'reference_id' => [
                        'description' => 'Parcel Id',
                        'data' => 'SampleCode1337-01',
                    ],
                ],
            ],
            'origin' => [
                'address' => [
                    'address_1' => '132 Commercial Street',
                    'postcode' => 'E1 6AZ',
                    'city' => 'London',
                    'country_code' => 'GB',
                ],
                'contact' => [
                    'name' => 'John Doe',
                    'phone_number' => '+46700000000',
                    'email' => 'John.Doe@example.org',
                ]
            ],
            'destination' => [
                'address' => [
                    'name' => 'Acme Corp',
                    'address_1' => '6 Fairclough Street',
                    'postcode' => 'E1 1PW',
                    'city' => 'London',
                    'country_code' => 'GB',
                ],
                'contact' => [
                    'name' => 'Jane Doe',
                    'phone_number' => '+46700000000',
                    'email' => ' Jane.Doe@example.org'
                ],
                'instructions' => [
                    'notes' => 'Lipsum',
                    'door_code' => '1234'
                ]
            ]
        ];
    }
}

// Let's get this show on the road :-)
Program::main();