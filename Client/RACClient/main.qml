import QtQuick 2.10
import QtQuick.Controls 2.3
import MqttClient 1.0
import Qt.labs.settings 1.0

ApplicationWindow {
    visible: true
    width: 400
    height: 600
    title: qsTr("Tabs")

    property string keyVal

    property var subscription

    Settings {
        id: settingValus
        property string keyVal
    }

    MqttClient {
        id: client
        hostname: "cig.nengxin.com.cn"
        port: 1883
        username: "nengxin"
        password: "NX@)!*"

        onConnected: {
            var topic = "/audiostate/" + keyVal;
            console.log(topic)
            subscription = client.subscribe(topic)
            subscription.messageReceived.connect(arrivedMessage)
            getVolume()
        }
    }

    function publishTopic() {
        return "/audioctrl/"+keyVal
    }

    // 增大音量
    function increaseVolume() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"VOLUME_UP\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }
    // 减小音量
    function decreaseVolume() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"VOLUME_DOWN\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }
    // 静音
    function muteToggle() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"MUTE_TOGGLE\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function playNext() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"MEDIA_NEXT\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function playPrev() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"MEDIA_PREVIOUS\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function mediaPlay() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"MEDIA_PLAY\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function mediaStop() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"MEDIA_STOP\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function getVolume() {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"GET_VOLUME\","+"\"Volume\":0}"
        client.publish(publishTopic(), json)
    }

    function setVolume(val) {
        var json = "{\"Key\":\""+keyVal+"\",\"Action\":\"SET_VOLUME\","+"\"Volume\":"+val+"}"

        client.publish(publishTopic(), json)
    }

    function arrivedMessage(payload)
    {
        var cn = Qt.locale("")
        ctrlPage.volumeDial.value = parseInt(payload)
    }

    SwipeView {
        id: swipeView
        anchors.fill: parent
//        currentIndex: tabBar.currentIndex

//        Page1Form {
        //        }

        Page2Form {
            id: ctrlPage
            Component.onCompleted: {
                keyEdit.text = settingValus.keyVal
            }
            volumeDial.onValueChanged: {
                setVolume(volumeDial.value.toFixed(0))
                volumeVal.text = volumeDial.value.toFixed(0)
            }
            stopBtn.onClicked: {
                mediaStop()
            }
            playBtn.onClicked: {
                mediaPlay()
            }
            prevBtn.onClicked: {
                playPrev()
            }
            nextBtn.onClicked: {
                playNext()
            }
            muteBtn.onClicked: {
                muteToggle()
            }
            decreaseVol.onClicked: {
                decreaseVolume()
                volumeDial.value -= 5
            }
            increaseVol.onClicked: {
                increaseVolume()
                volumeDial.value += 5
            }
            connectBtn.onClicked: {
                client.connectToHost()
                keyVal = keyEdit.text
                settingValus.keyVal = keyVal

            }

            client: client
        }
    }

//    footer: TabBar {
//        id: tabBar
//        currentIndex: swipeView.currentIndex

//        TabButton {
//            text: qsTr("Page 1")
//        }
//        TabButton {
//            text: qsTr("Page 2")
//        }
//    }
}
