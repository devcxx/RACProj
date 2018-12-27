/*
 * QML Material - An application framework implementing Material Design.
 *
 * Copyright (C) 2016 Michael Spencer <sonrisesoftware@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

#include "units.h"

#include <QGuiApplication>
#include <QQuickItem>

#if defined(Q_OS_ANDROID)
#include <QtAndroidExtras>
#endif

#define DEFAULT_DPI 72

#if defined (Q_OS_ANDROID)
static qreal s_dp = 1;
static qreal s_dpi = 72;
static bool s_isTablet = false;
#endif

UnitsAttached::UnitsAttached(QObject *attachee)
        : QObject(attachee), m_screen(nullptr), m_window(nullptr), m_dpi(0), m_multiplier(1)
{
    m_attachee = qobject_cast<QQuickItem *>(attachee);

    if (m_attachee) {
        if (m_attachee->window()) // It might not be assigned to a window yet
            windowChanged(m_attachee->window());
    } else {
        QQuickWindow *window = qobject_cast<QQuickWindow *>(attachee);
        if (window)
            windowChanged(window);
    }

    if (!m_screen)
        screenChanged(QGuiApplication::primaryScreen());
}

void UnitsAttached::windowChanged(QQuickWindow *window)
{
    if (m_window)
        disconnect(m_window, &QQuickWindow::screenChanged, this, &UnitsAttached::screenChanged);

    m_window = window;
    screenChanged(window ? window->screen() : nullptr);

    if (window)
        connect(window, &QQuickWindow::screenChanged, this, &UnitsAttached::screenChanged);
}

void UnitsAttached::screenChanged(QScreen *screen)
{
    if (screen != m_screen) {
        QScreen *oldScreen = m_screen;
        m_screen = screen;

        if (oldScreen)
            oldScreen->disconnect(this);

        if (oldScreen == nullptr || screen == nullptr ||
            screen->physicalDotsPerInch() != oldScreen->physicalDotsPerInch() ||
            screen->logicalDotsPerInch() != oldScreen->logicalDotsPerInch() ||
            screen->devicePixelRatio() != oldScreen->devicePixelRatio()) {
            updateDPI();
            emit dpChanged();
        }
    }
}

int UnitsAttached::dp() const
{
#if defined(Q_OS_ANDROID)
    return s_dp;
#endif
#if QT_VERSION >= QT_VERSION_CHECK(5, 6, 0)
    return m_multiplier;
#else
    auto dp = dpi() / 160;

    return dp > 0 ? dp : m_multiplier;
#endif
}

int UnitsAttached::dpi() const {
#if defined(Q_OS_ANDROID)
    return s_dpi;
#else
    return m_dpi;
#endif
}

qreal UnitsAttached::multiplier() const { return m_multiplier; }

void UnitsAttached::setMultiplier(qreal multiplier)
{
    if (m_multiplier != multiplier) {
        m_multiplier = multiplier;
        emit multiplierChanged();
    }
}

void UnitsAttached::updateDPI()
{
    if (m_screen == nullptr) {
        m_dpi = DEFAULT_DPI;
        return;
    }

#if defined(Q_OS_IOS)
    // iOS integration of scaling (retina, non-retina, 4K) does itself.
    m_dpi = m_screen->physicalDotsPerInch();
#elif defined(Q_OS_ANDROID)
    // https://bugreports.qt-project.org/browse/QTBUG-35701
    // recalculate dpi for Android

    QAndroidJniEnvironment env;
    QAndroidJniObject activity = QtAndroid::androidActivity();
    QAndroidJniObject resources =
            activity.callObjectMethod("getResources", "()Landroid/content/res/Resources;");
    if (env->ExceptionCheck()) {
        env->ExceptionDescribe();
        env->ExceptionClear();

        m_dpi = DEFAULT_DPI;
        return;
    }

    QAndroidJniObject displayMetrics =
            resources.callObjectMethod("getDisplayMetrics", "()Landroid/util/DisplayMetrics;");
    if (env->ExceptionCheck()) {
        env->ExceptionDescribe();
        env->ExceptionClear();

        m_dpi = DEFAULT_DPI;
        return;
    }
    m_dpi = displayMetrics.getField<int>("densityDpi");
    m_multiplier = displayMetrics.getField<float>("density");

//    QAndroidJniObject qtActivity =    QAndroidJniObject::callStaticObjectMethod("org/qtproject/qt5/android/QtNative", "activity", "()Landroid/app/Activity;");
//    QAndroidJniObject resources = qtActivity.callObjectMethod("getResources","()Landroid/content/res/Resources;");
//    QAndroidJniObject displayMetrics = resources.callObjectMethod("getDisplayMetrics","()Landroid/util/DisplayMetrics;");
//    int density = displayMetrics.getField<int>("densityDpi");
//    m_dpi = density;
//    m_multiplier = displayMetrics.getField<float>("density");

#else
    // standard dpi
    m_dpi = m_screen->logicalDotsPerInch() * m_screen->devicePixelRatio();
#endif
}


#if defined(Q_OS_ANDROID)
static void init() {

#if defined(Q_OS_IOS)
    // iOS integration of scaling (retina, non-retina, 4K) does itself.
    s_dpi = qApp->primaryScreen()->physicalDotsPerInch();

#elif defined(Q_OS_ANDROID)
        QAndroidJniObject activity = QAndroidJniObject::callStaticObjectMethod("org/qtproject/qt5/android/QtNative", "activity", "()Landroid/app/Activity;");
        QAndroidJniObject resource = activity.callObjectMethod("getResources","()Landroid/content/res/Resources;");
        QAndroidJniObject metrics = resource.callObjectMethod("getDisplayMetrics","()Landroid/util/DisplayMetrics;");
        s_dp = metrics.getField<float>("density");
        s_dpi = metrics.getField<int>("densityDpi");

#if (QT_VERSION >= QT_VERSION_CHECK(5, 6, 0))
        QGuiApplication *app = qobject_cast<QGuiApplication*>(QGuiApplication::instance());
        if (app->testAttribute(Qt::AA_EnableHighDpiScaling)) {
            s_dp = s_dp / app->devicePixelRatio();
            s_dpi = s_dpi / app->devicePixelRatio();
        }
#endif

        /* Is Tablet. Experimental code */

        QAndroidJniObject configuration = resource.callObjectMethod("getConfiguration","()Landroid/content/res/Configuration;");
        int screenLayout = configuration.getField<int>("screenLayout");
        int SCREENLAYOUT_SIZE_MASK = QAndroidJniObject::getStaticField<int>("android/content/res/Configuration","SCREENLAYOUT_SIZE_MASK");
        int SCREENLAYOUT_SIZE_LARGE = QAndroidJniObject::getStaticField<int>("android/content/res/Configuration","SCREENLAYOUT_SIZE_LARGE");

        s_isTablet = (screenLayout & SCREENLAYOUT_SIZE_MASK) >= SCREENLAYOUT_SIZE_LARGE;

#else
    // standard dpi
    s_dpi = qApp->primaryScreen()->logicalDotsPerInch()*qApp->primaryScreen()->devicePixelRatio();
#endif

//    qmlRegisterSingletonType<QADevice>("QuickAndroid", 0, 1, "Device", provider);
}

Q_COREAPP_STARTUP_FUNCTION(init)
#endif
