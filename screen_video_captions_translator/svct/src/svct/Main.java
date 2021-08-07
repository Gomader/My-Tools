/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package svct;

/**
 *
 * @author gonghe
 */
public class Main {

    public static MainForm form = new MainForm();
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        // TODO code application logic here
        
        showMainForm(args);
    }
    
    static protected void showMainForm(String[] args){
        form.main(args);
    }
    
    
}
