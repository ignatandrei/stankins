const PROXY_CONFIG = [
    {
      context: [ 
          "/api",  
          "/swagger"              
      ],
      target: "http://stankins_run_container:5000",
      secure: false,
      "changeOrigin": true,
      "logLevel": "debug",
      "ws":true
    }
]
module.exports = PROXY_CONFIG;
