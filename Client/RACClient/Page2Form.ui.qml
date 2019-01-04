import QtQuick 2.10
import QtQuick.Controls 2.3
import MqttClient 1.0
import Units 0.3

Page {
    width: 400* Units.dp
    height: 600* Units.dp
    property alias volumeVal: volumeVal
    property alias volumeDial: volumeDial
    property alias decreaseVol: decreaseVol
    property alias stopBtn: stopBtn
    property alias playBtn: playBtn
    property alias prevBtn: prevBtn
    property alias nextBtn: nextBtn
    property alias muteBtn: muteBtn
    property alias increaseVol: increaseVol
    property alias keyEdit: keyEdit
    property alias connectBtn: connectBtn

    property var client

    header: Label {
        text: qsTr("Remote Audio Controller")
        font.pixelSize: Qt.application.font.pixelSize * 2
        padding: 10* Units.dp
    }

    Dial {
        id: volumeDial
        x: 45* Units.dp
        y: 165* Units.dp
        width: 200* Units.dp
        height: 200* Units.dp
        wheelEnabled: true
        stepSize: 5
        to: 100
    }

    Button {
        id: increaseVol
        x: 270* Units.dp
        y: 160* Units.dp
        width: 50 * Units.dp
        height: 50 * Units.dp
        text: qsTr("︿")
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: decreaseVol
        x: 270* Units.dp
        y: 290* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr("﹀")
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: prevBtn
        x: 40* Units.dp
        y: 420* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr("<<")
        font.weight: Font.ExtraLight
        focusPolicy: Qt.WheelFocus
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: playBtn
        x: 105* Units.dp
        y: 420* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr(">|")
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: stopBtn
        x: 175* Units.dp
        y: 420* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr("||")
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: nextBtn
        x: 240* Units.dp
        y: 420* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr(">>")
        font.pixelSize: 15*Units.dp
    }

    Button {
        id: muteBtn
        x: 220* Units.dp
        y: 90* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr("Mute")
        font.pixelSize: 15*Units.dp
    }

    Text {
        id: volumeVal
        x: 120* Units.dp
        y: 240* Units.dp
        width: 50* Units.dp
        height: 50* Units.dp
        text: qsTr("0")
        font.bold: false
        font.italic: false
        horizontalAlignment: Text.AlignHCenter
        verticalAlignment: Text.AlignVCenter
        font.pixelSize: 50*Units.dp
    }

    TextField {
        id: keyEdit
        x: 30* Units.dp
        y: 10* Units.dp
        width: 195* Units.dp
        height: 50* Units.dp
        inputMethodHints: Qt.ImhDigitsOnly
        placeholderText: "Key value"
        font.pixelSize: 20*Units.dp
    }

    Button {
        id: connectBtn
        checkable: true
        x: 250* Units.dp
        y: 10* Units.dp
        width: 100* Units.dp
        height: 50* Units.dp
        font.pixelSize: 15*Units.dp
        text: client.state === MqttClient.Connected ? "Disconnect" : "Connect"
    }
}
