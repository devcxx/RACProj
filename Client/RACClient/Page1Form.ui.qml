import QtQuick 2.10
import QtQuick.Controls 2.3

Page {
    width: 600
    height: 400

    header: Label {
        text: qsTr("Page 1")
        font.pixelSize: Qt.application.font.pixelSize * 2
        padding: 10
    }

    TextField {
        id: textField
        x: 101
        y: 36
        width: 199
        height: 37
        text: qsTr("Text Field")
    }

    Button {
        id: button
        x: 324
        y: 45
        text: qsTr("Button")
    }

    Text {
        id: text1
        x: 101
        y: 137
        width: 128
        height: 38
        text: qsTr("Text")
        font.pixelSize: 12
    }

    BusyIndicator {
        id: busyIndicator
        x: 123
        y: 127
        width: 171
        height: 127
    }
}
