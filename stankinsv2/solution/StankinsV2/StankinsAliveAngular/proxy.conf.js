const PROXY_CONFIG = [
    {
      context: [
          "/DataHub" , 
          "/api",                
      ],
      target: "https://localhost:44334/",
      secure: false,
      "changeOrigin": true,
      "logLevel": "debug",
      "ws":true
    }
]
module.exports = PROXY_CONFIG;
