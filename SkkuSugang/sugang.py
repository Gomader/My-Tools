from time import sleep
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from random import random

def sendRequests(browser):
    browser.switch_to.frame("contentFrame")
    browser.switch_to.frame("topFrame")
    browser.execute_script("javascript:goMenu('1');")
    sleep(5)
    browser.switch_to.default_content()
    browser.switch_to.frame("Main")
    browser.switch_to.frame("contentFrame")
    browser.switch_to.frame("mainFrame")
    while True:
        for i in range(3):
            browser.execute_script("choice('{}','N')".format(i+1))
            sleep(5+random()*10)
        sleep(5+random()*20)

def login(browser):
    browser.get(url)
    browser.delete_all_cookies()
    sleep(2)
    browser.switch_to.frame("Main")
    browser.find_element_by_name("id").send_keys("2018313592")
    browser.find_element_by_name("pwd").send_keys("q#3q662d")
    browser.find_element_by_id("btn_login").click()
    sleep(5+random()*5)

def main():
    browser = webdriver.Chrome(options=chrome_options)
    login(browser)
    sendRequests(browser)

if __name__ == "__main__":
    url = "http://sugang.skku.edu/skku/"
    chrome_options = Options()
    chrome_options.add_experimental_option("detach", True)
    chrome_options.add_argument('--no-sandbox')
    chrome_options.add_argument('--disable-dev-shm-usage')
    chrome_options.add_argument('--headless')
    chrome_options.add_experimental_option('excludeSwitches', ['enable-logging'])
    main()