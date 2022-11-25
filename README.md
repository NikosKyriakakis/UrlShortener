# Url Shortener

This is an implementation of a URL shortening service that acts as the backend and data management portion of such a project. Therefore, no link redirection is provided as it is assumed that some other service implements that part. 

The URL shortening service is responsible for exposing a REST API which supports POST, GET, DELETE operations on a MongoDB database, where a long URL is replaced by a short version and is returned along with other useful info ( e.g., expirationDate, ...). Embedded in this service is a machine learning model trained on the Kaggle URL dataset (available here: https://www.kaggle.com/datasets/sid321axn/malicious-urls-dataset), which classifies incoming URLs as malicious, phising, benign, or defacement and stores this info so it can be retrieved by a consuming process. The selected model is a Maximum Entropy Multiclass estimator using the LBFGS optimizer.

The other main service is the DB management, which is responsible for periodically querying the database and deleting expired URLs.

Below is a simple diagram of the system's architecture

<img title="URL Shortener Services" src="/Images/URL_Shortener_Backend.jpg">
