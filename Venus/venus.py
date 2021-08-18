from pywinauto.application import Application
from pywinauto import mouse
from pywinauto.keyboard import send_keys
from xlrd import open_workbook
from wx import App,Dialog,EVT_BUTTON,NewIdRef,EVT_CLOSE,TextCtrl,TE_PROCESS_ENTER,Button,MessageBox,OK
from wx.lib.filebrowsebutton import FileBrowseButtonWithHistory
from sys import exit

class MyApp(Dialog):

    app = None

    row = 0

    index = 0

    sheet = None

    title = "Venus"

    def __init__(self):

        Dialog.__init__(self,None,-1,self.title,size=(200,250))

        self.filePath = FileBrowseButtonWithHistory(self,labelText="File",pos=(5,20),size=(180,30))

        self.windowPath = TextCtrl(self,-1,"Untitled.net - Venus",pos=(10,70),size=(165,25),style=TE_PROCESS_ENTER)

        self.startFrom = TextCtrl(self,-1,"Start from (index)",pos=(10,120),size=(165,25),style=TE_PROCESS_ENTER)

        self.start = Button(self,-1,"START",pos=(42,170),size=(100,-1))
        self.Bind(EVT_BUTTON,self.onStart,self.start)

        self.Bind(EVT_CLOSE, self.onExit)

    def onStart(self,event):
        windowsName = self.windowPath.GetValue()
        try:
            if self.start.GetLabel() == "START":
                self.index = int(self.startFrom.GetValue()) - 1
        except:
            pass
        try:
            self.sheet = open_workbook(self.filePath.GetValue()).sheets()[0]
            self.row = self.sheet.nrows
            self.SetTitle(self.title+" - {}/{}".format(self.index+1,self.row))
            try:
                self.app = Application(backend="uia").connect(title_re=windowsName)
                self.app[windowsName].maximize()
                self.app[windowsName]["Toolbar"].CheckBox3.click()
                while self.index != self.row:
                    mouse.click(coords=(1000,1000))
                    try:
                        self.app[windowsName]["Dialog2"].Edit0.set_edit_text(str(self.sheet.cell(self.index,1).value))
                        self.app[windowsName]["Dialog2"].Edit2.set_edit_text(str(self.sheet.cell(self.index,2).value))
                        self.app[windowsName]["Dialog2"].Edit3.set_edit_text(str(self.sheet.cell(self.index,3).value))
                        self.app[windowsName]["Dialog2"].Button0.click()
                        self.index += 1
                        send_keys('^s')
                    except:
                        self.SetTitle(self.title+" - {}/{}".format(self.index+1,self.row))
                        self.start.SetLabel("CONTINUE")
                        MessageBox("Finished {}/{}.\nYou can continue.".format(self.index+1,self.row),"Error",OK)
                        break
                if self.index == self.row:
                    MessageBox("All Data inputed!","Finish",OK)
                    self.getZero()
            except:
                MessageBox("Venus windows does not exist!","Error",OK)
        except:
            MessageBox("Excel file does not exist!","Error",OK)

    def getZero(self):
        self.app = None
        self.row = 0
        self.index = 0
        self.sheet = None
        self.title = "Venus"
        self.start.SetLabel("START")

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