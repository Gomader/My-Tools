from pywinauto.application import Application
from pywinauto import mouse
from xlrd import open_workbook
from wx import App,Frame,Dialog,EVT_BUTTON,NewIdRef,EVT_CLOSE,TextCtrl,TE_PROCESS_ENTER,Button
from wx.lib.filebrowsebutton import FileBrowseButtonWithHistory
from sys import exit

def inputCode(windowName,fileName):
    
    app = Application(backend="uia").connect(title_re=windowName)

    app[windowName].maximize()
    app[windowName]["Toolbar"].CheckBox3.click()

    sheet = open_workbook(fileName).sheets()[0]

    for row in range(sheet.nrows):

        mouse.click(coords=(1000,1000))

        app[windowName]["Dialog2"].Edit0.set_edit_text(str(sheet.cell(row,1).value))
        app[windowName]["Dialog2"].Edit2.set_edit_text(str(sheet.cell(row,2).value))
        app[windowName]["Dialog2"].Edit3.set_edit_text(str(sheet.cell(row,3).value))
        
        app[windowName]["Dialog2"].Button0.click()

class MyApp(Dialog):

    def __init__(self):

        Dialog.__init__(self,None,-1,"Venus Data Entry",size=(200,200))

        self.filePath = FileBrowseButtonWithHistory(self,labelText="File",pos=(5,20),size=(180,30))

        self.windowPath = TextCtrl(self,-1,"Untitled - Venus",pos=(10,70),size=(165,25),style=TE_PROCESS_ENTER)

        self.start = Button(self,-1,"START",pos=(42,120),size=(100,-1))
        self.Bind(EVT_BUTTON,self.onStart,self.start)

        self.Bind(EVT_CLOSE, self.onExit)

    def onStart(self,event):
        inputCode(self.windowPath.GetValue(),self.filePath.GetValue())

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