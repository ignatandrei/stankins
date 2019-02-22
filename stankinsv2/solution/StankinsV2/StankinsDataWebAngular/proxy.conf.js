const PROXY_CONFIG = [
    {
      context: [ 
          "/api",                
      ],
      target: "http://localhost:53385",
      secure: false,
      "changeOrigin": true,
      "logLevel": "debug",
      "ws":true
    }
]
module.exports = PROXY_CONFIG;
