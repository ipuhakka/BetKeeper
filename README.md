# Description
BetKeeper is a system to log 1X2 bet statistics. It enables users to log their bets and results into different folders, so that users can analyze different betting styles and their success. 
This repository contains build results , instructions to install react client,and a small demo database to demo the app.

# Requirements

## Client
Machine needs to have installed [node.js](https://nodejs.org/en/) in order to install the necessary dependancies.

## API & database
To build any made changes machine needs to have[.NET framework](https://www.microsoft.com/net/download) in order to build changes. 
Repository contains all .dll's and executables to run the program as it is, so building these are not necessary to test the app.

# Build
Client application can be built by opening command prompt in Client folder, and typing command `npm install`.
Database, api and tests can be built using visual studio.

# Run

## API 
API can be started by locating the `API\bin\debug` or 
`API\bin\release` folder and running the API.exe as admin.

## Client
Client can be started by going to `Client` folder on console and typing command `npm start`.