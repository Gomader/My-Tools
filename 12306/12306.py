import requests, json, time, random

url = "https://kyfw.12306.cn/otn/leftTicket/queryZ?leftTicketDTO.train_date=2022-10-07&leftTicketDTO.from_station=AXT&leftTicketDTO.to_station=IFP&purpose_codes=ADULT"

sleep_time = 1.5

headers = {
    "Cookie": "_uab_collina=166480564638539954253574; JSESSIONID=8DE003CDA918F865D4F9C58064B9E433; BIGipServerotn=485491210.38945.0000; route=c5c62a339e7744272a54643b3be5bf64; fo=rks5cpcpb8km9rbgzdYMOfFrd5tKTE6Hnwywut65ZAdVGMbNUJZ_-Jd0f-txfiQnCwCq0z3OMgYxL_RTZ8rf85Bua7UXsAclj_Sv12f1bh7r1k1dMnQoHOgY_QYAoWefqPa7XnvosjgZG0A4DR_GCDvF52zwj9H17G1TkjMl60XW535qc3Is4_tlPwayDxBcxt0044FMAsmILogm; BIGipServerpool_passport=165937674.50215.0000; RAIL_EXPIRATION=1665097105879; RAIL_DEVICEID=W-ir7wkSWmc5EuWhLYk55k_T-QRu3PL8L2uFLB-T13Ni6VNiguTVg5sqkYeYx5suQInwZI7wHArUp2JsTQwdbohX7zq6J9v0mgcjXNp6poBzf50fFTTM8reS91Voe1r1nlqa8BYlA2H_47ow0Wipl-NxUjwZeQKB; guidesStatus=off; highContrastMode=defaltMode; cursorStatus=off; _jc_save_fromStation=%u978D%u5C71%u897F%2CAXT; _jc_save_toStation=%u5317%u4EAC%u671D%u9633%2CIFP; _jc_save_fromDate=2022-10-07; _jc_save_toDate=2022-10-03; _jc_save_wfdc_flag=dc; BIGipServerportal=2949906698.17183.0000",
    "User-Agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"
}

def pushToWechat(pushplus_content):
    pushplus_token = "3f143732adb648db88f3e02883b9c2e4"
    pushplus_title = "快去抢票"
    pushplus_url = f"https://www.pushplus.plus/send?token={pushplus_token}&title={pushplus_title}&content={pushplus_content}&template=html"
    result = requests.get(url=pushplus_url)

def searching():
    loop_times = 0
    while True:
        loop_times += 1
        response = requests.get(url=url, headers=headers)
        try:
            result = json.loads(response.text).get("data").get("result")[0]
            result = result.split("|")[-19:-16]
            print(f'已查询{loop_times}次, ')
            if len(set(result)) != 1 or '无' not in result:
                pushToWechat(f'发现空座，共查询{loop_times}次, msg: {result}')
                print(f'发现空座，共查询{loop_times}次, msg: {result}')
        except Exception as e:
            print(f'error: {e}, response: {response.text}')
        time.sleep(sleep_time + sleep_time * random.random())

def main():
    searching()

if __name__ == '__main__':
    main()