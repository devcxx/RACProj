#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include <mqtt/qmlmqttclient.h>


#include "core/device.h"
#include "core/units.h"

int main(int argc, char *argv[])
{
#if defined(Q_OS_WIN)
    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);
#endif

    QGuiApplication app(argc, argv);

    QCoreApplication::setApplicationName("Remote Audio Controller");
    QCoreApplication::setOrganizationName("Guangzhou Nengxin Cultural Technology Co., Ltd.");
    QCoreApplication::setOrganizationDomain("cn.com.nengxin");

    QQmlApplicationEngine engine;

    qmlRegisterType<QmlMqttClient>("MqttClient", 1, 0, "MqttClient");
    qmlRegisterUncreatableType<QmlMqttSubscription>("MqttClient", 1, 0, "MqttSubscription", QLatin1String("Subscriptions are read-only"));

    qmlRegisterSingletonType<Device>("Device", 0, 1, "Device", Device::qmlSingleton);
    qmlRegisterUncreatableType<Units>("Units", 0, 3, "Units", QStringLiteral("Units can only be used via the attached property."));

    engine.load(QUrl(QStringLiteral("qrc:/main.qml")));
    if (engine.rootObjects().isEmpty())
        return -1;

    return app.exec();
}
