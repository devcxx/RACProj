QT += quick core network

CONFIG += c++11

QT += core-private

android {
    QT += androidextras
}

# The following define makes your compiler emit warnings if you use
# any feature of Qt which as been marked deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0


PUBLIC_HEADERS += \
    core/units.h \
    core/device.h \
    mqtt/qmqttglobal.h \
    mqtt/qmqttclient.h \
    mqtt/qmqttmessage.h \
    mqtt/qmqttsubscription.h \
    mqtt/qmqtttopicfilter.h \
    mqtt/qmqtttopicname.h \
    mqtt/qmlmqttclient.h \

PRIVATE_HEADERS += \
    mqtt/qmqttclient_p.h \
    mqtt/qmqttconnection_p.h \
    mqtt/qmqttcontrolpacket_p.h \
    mqtt/qmqttsubscription_p.h

SOURCES += \
    core/units.cpp \
    core/device.cpp \
    mqtt/qmqttclient.cpp \
    mqtt/qmqttconnection.cpp \
    mqtt/qmqttcontrolpacket.cpp \
    mqtt/qmqttmessage.cpp \
    mqtt/qmqttsubscription.cpp \
    mqtt/qmqtttopicfilter.cpp \
    mqtt/qmqtttopicname.cpp \
    mqtt/qmlmqttclient.cpp \
    main.cpp \

HEADERS += $$PUBLIC_HEADERS $$PRIVATE_HEADERS

RESOURCES += qml.qrc

# Additional import path used to resolve QML modules in Qt Creator's code model
QML_IMPORT_PATH =

# Additional import path used to resolve QML modules just for Qt Quick Designer
QML_DESIGNER_IMPORT_PATH =

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

NDK_ROOT = F:/crystax-ndk-10.3.2
message($$NDK_ROOT)
message($$ANDROID_TARGET_ARCH)
android{
    INCLUDEPATH *=  \
        $$NDK_ROOT/sources/crystax/include \
#        $$NDK_ROOT/sources/crystax/include/crystax \

    LIBS *= \
        -L$$NDK_ROOT/sources/crystax/libs/$$ANDROID_TARGET_ARCH/ \
}

contains(ANDROID_TARGET_ARCH,armeabi-v7a) {
    ANDROID_EXTRA_LIBS = \
       F:/crystax-ndk-10.3.2/sources/crystax/libs/$$ANDROID_TARGET_ARCH/libcrystax.so
}

DISTFILES += \
    android/AndroidManifest.xml \
    android/gradle/wrapper/gradle-wrapper.jar \
    android/gradlew \
    android/res/values/libs.xml \
    android/build.gradle \
    android/gradle/wrapper/gradle-wrapper.properties \
    android/gradlew.bat

ANDROID_PACKAGE_SOURCE_DIR = $$PWD/android
