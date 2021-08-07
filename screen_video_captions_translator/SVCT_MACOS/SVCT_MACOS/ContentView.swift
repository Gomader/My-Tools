//
//  ContentView.swift
//  SVCT_MACOS
//
//  Created by 宫赫 on 2021/4/14.
//

import SwiftUI

struct ContentView: View {
    var body: some View {
        Text("Hello, world!")
            .padding()
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        Group {
            ContentView()
        }
        .frame(width: 200.0, height: 200.0)
    }
}
