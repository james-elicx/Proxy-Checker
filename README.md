# Proxy Checker

![GitHub](https://img.shields.io/github/license/moodiest/Proxy-Checker?style=flat-square)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/moodiest/Proxy-Checker?style=flat-square)
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat-square)](https://github.com/moodiest/Proxy-Checker/issues)
[![HitCount](http://hits.dwyl.com/moodiest/Proxy-Checker.svg)](http://hits.dwyl.com/moodiest/Proxy-Checker)
 
This tool takes an input for a list of proxies and then checks each one by making a request to api.ipify.com. It features multithreading and a custom timeout (milliseconds). Currently, only HTTP proxies are supported.

Contributors are welcome. It was created using C# with the .NET Core framework.

## Features
* CLI (command-line interface)
* HTTP proxy support
* Multithreading
* Custom Timeout

## To-Do
* GUI (graphical interface)
* Socks4/Socks5 proxy support

## Usage

Example: `Proxy-Checker_CLI --file http_proxies.txt --output working_proxies.txt --threads 250 --timeout 1000`

```
Usage: Proxy-Checker_CLI [OPTIONS]

Options:
  -f, --file=VALUE           A text file containing proxies to check e.g. '--
                               file proxies.txt'
  -o, --output=VALUE         Text file to output working proxies to. If a file
                               already exists, working proxies will be appended
                               to the end of the file. e.g. '--output working_
                               proxies.txt' DEFAULT: working_proxies.txt
  -t, --threads=VALUE        Number of threads to use e.g. '--threads 10' (
                               DEFAULT: 1)
  -x, --timeout=VALUE        Timeout when testing proxies in milliseconds e.g. '
                               --timeout 1000' DEFAULT: 2000
  -h, --help                 Show this message and exit e.g. '--help'
```

## Screenshots

![CLI Screenshot](https://i.imgur.com/Kykk8Qy.png)
