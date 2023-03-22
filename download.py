import requests
from datetime import datetime

header = {

    "authority": "khu-storage.commonscdn.com",
    "method": "GET",
    "path": "/contents3/khu1000001/629315d374a36/contents/media_files/screen.mp4?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NTQzMTE1OTgsInBhdGgiOiJcL2NvbnRlbnRzM1wva2h1MTAwMDAwMVwvNjI5MzE1ZDM3NGEzNlwvY29udGVudHNcL21lZGlhX2ZpbGVzXC9zY3JlZW4ubXA0In0.AF63Y3ze4z8ZRwCee6n1bZqwk4Zdw4Kvw8Fuhms_igI",
    "scheme": "https",
    "accept-encoding": "identity;q=1, *;q=0",
    "accept": "*/*",
    "accept-encoding": "identity;q=1, *;q=0",
    "accept-language": "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7,zh-CN;q=0.6,ko;q=0.5",
    "cache-control": "no-cache",
    "pragma": "no-cache",
    "referer": "https://commons.khu.ac.kr/",
    "sec-ch-ua": '" Not A;Brand";v="99", "Chromium";v="102", "Google Chrome";v="102"',
    "sec-ch-ua-mobile": "?0",
    "sec-ch-ua-platform": '"macOS"',
    "sec-fetch-dest": "video",
    "sec-fetch-mode": "no-cors",
    "sec-fetch-site": "cross-site",
    "user-agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36"

}

url = "https://" + header["authority"] + header["path"]

videoRange = 0

def makeHeader(bytes):
    header["range"] = "bytes={}-".format(bytes)

def main():
    startTime = datetime().now()
    f = open("./output.mp4", "wb")
    makeHeader(0)
    req = requests.get(url, headers=header, stream=True)
    print(req.headers["Content-Length"])

    length = int(req.headers["Content-Length"])
    i = 0

    for chunk in req.iter_content(chunk_size=1024):
        if chunk:
            i += len(chunk)
            print("{}% - {}".format((i/length)*100, datetime.now() - startTime), end="\r")
            f.write(chunk)

    f.close()
    print(datetime.now() - startTime)

if __name__ == '__main__':
    main()