<?php

require_once '../vendor/autoload.php';

use GuzzleHttp\Client;
use GuzzleHttp\Exception\RequestException;
use GuzzleHttp\Exception\GuzzleException;

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
        echo("Create delivery 📦\n");
        try {
            $shipmentInformation = $this->createShipment();
            $shipmentNumber = $shipmentInformation[0];
            $trackingNumber = $shipmentInformation[1];
            echo(">> Done!\n");
            echo(">> shipment_number: $shipmentNumber\n");
            echo(">> tracking_number: $trackingNumber\n");
            echo("\n");
            echo("Get shipping label 🏷️\n");
            $this->shippingLabel($trackingNumber);
            echo(">> Done! 🌟\n");
            echo(">> ZPL saved in 'output' folder\n");
            echo("\n");
            echo("Bye bye 👋\n");
        } catch (Exception $e) {
            echo("An error occurred: " . $e->getMessage());
        }
    }

    /**
     * @throws Exception
     */
    private function createShipment(): array
    {
        $data = $this->getShipmentData();

        $body = json_encode($data);
        try {
            $response = self::$httpClient->post('shipments', [
                'headers' => [
                    'Content-Type' => 'application/json',
                ],
                'body' => $body,
            ]);
        } catch (RequestException $e) {
            throw new Exception("Error! " . $e->getMessage());
        } catch (GuzzleException $e) {
            throw new Exception("Error! " . $e->getMessage());
        }

        if ($response->getStatusCode() !== 201) {
            throw new Exception("Error! " . $response->getStatusCode());
        }

        $json = json_decode($response->getBody(), true);
        $shipmentNumber = $json['shipment_number'];
        $deliveries = $json['deliveries'];
        $trackingNumber = $deliveries[0]['tracking_number'];

        return [$shipmentNumber, $trackingNumber];
    }

    /**
     * @throws Exception
     */
    private function shippingLabel(string $trackingNumber): void
    {
        try {
            $response = self::$httpClient->get("deliveries/$trackingNumber/shipping-label");
        } catch (RequestException $e) {
            throw new Exception("Error! " . $e->getMessage());
        } catch (GuzzleException $e) {
            throw new Exception("Error! " . $e->getMessage());
        }

        if ($response->getStatusCode() !== 200) {
            throw new Exception("Error! " . $response->getStatusCode());
        }

        $output = $response->getBody()->getContents();
        $path = __DIR__ . '/../output/' . $trackingNumber . ".zpl";
        file_put_contents($path, $output);
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