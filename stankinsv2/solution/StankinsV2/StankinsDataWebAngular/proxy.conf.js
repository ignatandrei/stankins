const PROXY_CONFIG = [
    {
      context: [ 
          "/api",  
          "/swagger"              
      ],
      target: "http://localhost:53385",
      secure: false,
      "changeOrigin": true,
      "logLevel": "debug",
      "ws":true
    }
]
module.exports = PROXY_CONFIG;
