# Url Shortener

This is an implementation of a URL shortening service that acts as the backend and data management portion of such a project. Therefore, no link redirection is provided as it is assumed that some other service implements that part. 

The URL shortening service is responsible for exposing a REST API which supports POST, GET, DELETE operations on a MongoDB database, where a long URL is replaced by a short version and is returned along with other useful info (expirationDate, ...). Embedded in this service is a machine learning model trained on the Kaggle URL dataset, which classifies incoming URLs as malicious, phising, benign, and stores this info so it can be retrieved by a consuming process.

The other main service is the DB management, and is responsible for periodically querying the database and deleting expired URLs.

Below is a simple diagram of the system's architecture

<img title="URL Shortener Services" src="/images/URL_Shortener_Backend.jpg">
