const express = require('express');
const http = require('http');
const app = express();
require('./routes')(app);

const port = 3000;

const server = http.createServer(app);

server.listen(port, () => console.log('Listening on port ' + port));

function close(){
  console.log("Shutting down server");
  app.close();
}

module.exports = app;
module.exports.close = close;
