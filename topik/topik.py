from selenium import webdriver
from bs4 import BeautifulSoup
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.select import Select
from time import sleep
from random import random
from wx import App,Frame,NewIdRef,EVT_CLOSE,TextCtrl,Button,EVT_BUTTON,Panel,MessageBox,OK,StaticText
from sys import exit
from threading import Thread

class MyApp(Frame):
    chrome_options = Options()
    chrome_options.add_experimental_option("detach", True)
    browser = None
    def __init__(self):
        Frame.__init__(self,None,-1,"TOPIK",size=(240,250))
        panel = Panel(self)
        self.username = TextCtrl(panel,-1,"Topik账号",pos=(20,20),size=(200,25))
        self.password = TextCtrl(panel,-1,"Topik密码",pos=(20,60),size=(200,25))
        self.loginBtn = Button(panel,-1,"点击登录",pos=(20,100),size=(200,25))
        self.Bind(EVT_BUTTON,self.onLogin,self.loginBtn)
        self.label1 = StaticText(panel,-1,"随机刷新间隔基数(秒):",pos=(20,142))
        self.sleeptime = TextCtrl(panel,-1,"5",pos=(170,140),size=(50,25))

        self.startBtn = Button(panel,-1,"点击开始",pos=(20,180),size=(200,25))
        self.Bind(EVT_BUTTON,self.onStart,self.startBtn)

        self.Bind(EVT_CLOSE, self.onExit)

    def onStart(self,event):
        self.browser.find_element_by_css_selector("a[id=\"btnSearch\"]").click()
        t = Thread(target=self.onRefresh)
        t.start()

    def onRefresh(self):
        while True:
            html = self.browser.page_source
            soup = BeautifulSoup(html,"html.parser")
            btnArgeeT2Icon52 = soup.find_all("a",{"id":"btnArgeeT2Icon52"})
            if len(btnArgeeT2Icon52) == 1:
                sleep(int(self.sleeptime.GetValue())*random())
                self.browser.refresh()
            else:
                break
        MessageBox("","有空位！！！！！！",OK)
    
    def onLogin(self,event):
        self.browser = webdriver.Chrome(options=self.chrome_options)
        self.browser.get("https://www.topik.go.kr/TWMBER/TWMBER0010.do")
        sleep(5)
        self.browser.find_element_by_css_selector("a[title=\"제78회 한국어능력시험(TOPIK) 접수하기\"]").click()
        sleep(5)
        self.browser.find_element_by_css_selector("input[title=\"아이디\"]").send_keys(self.username.GetValue())
        self.browser.find_element_by_css_selector("input[title=\"비밀번호\"]").send_keys(self.password.GetValue())
        self.browser.find_element_by_css_selector("a[class=\"btns btns_l btns_ok block\"]").click()
        MessageBox("若失败请自行登录\n在登录完成后选择好筛选条件后点击开始","登录过程结束",OK)

    def onExit(self,event):
        self.Destroy()
        exit(0)

def main():
    gui = App()
    guiWin = MyApp()
    guiWin.Centre()
    guiWin.Show()
    gui.MainLoop()

if __name__ == "__main__":
    
    main()