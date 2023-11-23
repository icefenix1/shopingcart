# shopingcart

## Setup
There is a docker compose file included in the solution. It sets up an environment that includes the following:

* sql server
* minio (s3 storage)
* key-cloak (Auth Provider)

Once the environment is up and running there is some setup that needs to be done.

### host file.

In your host file add the following line

```

127.0.0.1	minio.localhost keycloak-container cart.localhost

```

### MINIO

Default address http://minio.localhost

You will need to:

* Create a use and generate an access key for the user. Add this to the appsettings
* Create a bucket for the images to be pushed to. The app default is "images"

### key-cloak

Default address http://keycloak-container

* Go to the admin console.
* Create a client called ShopingCart
* Set "Access Type" to Confidential
* You can now get the SecretId from the credentials tab and put it in the appsettings

### SQL

SQL should sort it's self out at run time.

## Known Issues

These are the issues that I am aware of but ran out of time to resolve.

### Tests

There are 3 Test that don't work. 

They are due it issues with the way they are being MOQed.

### CORS

There is a CORS issue on the auth redirect in swagger. All the correct settings are there so this shouldn't be an issue but it persisted. 

I used a browser extension to test the auth flow.

### Auth

The Auth on the .net end isn't recognizing the auth that swagger is generating. Like the other issues I ran out of time to chase down the disconnect.

I removed the auth from the controllers so that process flow and logic can be demonstrated. Code was added to get the user information from the Token that is being passed.

## Future State

In the long term this should be split into 2 separate services. One for products and one for the shopping cart. This will make it possible to scale them individually. 
