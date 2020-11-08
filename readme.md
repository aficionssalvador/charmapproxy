# CharMapProxy

It is a proxy for telnet connections that allows the conversion of the character set with 8-bit character sets for example set IN2 to ANSI

It is a proxy based on the tcp layer of the proxy developed by https://github.com/Stormancer/netproxy

---

Es un proxy per connexions telnet que permet la conversió del set de caràcters amb sets de caracters de 8 bits per excemple set IN2 a ANSI

Es un proxy basado en la capa tcp del proxy desarrollado por https://github.com/Stormancer/netproxy

```
telnet client <--> CharMapProxy <--> telnet server
```

The configuration file follows the following format:

The source byte is "s" and the destination byte of the map is "d".

---

El fitxer de configuració segueix  el següent format:

El byte origen es "s" i el byte destí del mapeig es "d". 

```
 "telnet": {
    "localport": 9923,
    "protocol": "tcp",
    "forwardIp": "127.0.0.1",
    "forwardPort": 23,
    "client2server": [
      {
        "s": 0,
        "d": 0
      }
    ],
    "server2client": [
      {
        "s": 0,
        "d": 0
      }
    ]
  }
}
```

