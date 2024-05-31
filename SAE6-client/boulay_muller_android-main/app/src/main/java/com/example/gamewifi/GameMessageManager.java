package com.example.gamewifi;

import android.util.Log;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class GameMessageManager {
    private static Socket soc = null;
    private static int portConnect = 9999;
    private static String servAddr = String.valueOf("192.168.1.98");
    private static PrintWriter out;
    private static BufferedReader in;
    private static boolean log = false;

    public static void connect(String address)
    {
        setServerAddress(address);
        connect();
    }

    public static void connect(String address, int port)
    {
        setPort(port);
        connect(address);
    }

    public static void connect()
    {
        new Thread(){
            public void run() {
                try {
                    soc = new Socket(servAddr, portConnect);
                    if (soc != null)
                    {
                        out = new PrintWriter(soc.getOutputStream());
                        in = new BufferedReader(new InputStreamReader(soc.getInputStream()));
                    }

                } catch (IOException e) {
                    e.printStackTrace();
                }
            }}.start();
    }

    public static boolean isConnected()
    {
        if(soc != null && !soc.isClosed())
            return true;
        return false;
    }

    public static boolean sendMessage(String msg)
    {
        if(isConnected())
        {
            try {
                if(log)
                    Log.i("GameMessManager", "sending : " + msg);
                out.println(msg);
                out.flush();
                return true;
            } catch (Exception e) {
                if(log)
                    Log.i("GameMessManager",
                            "attempted sendMessage got exception : "
                                    + e.getMessage());
            }
        }
        else
        {
            if(log)
                Log.i("GameMessManager",
                        "attempted sendMessage: not connected");
        }
        return false;
    }

    public static String getNextMessage()
    {
        if(isConnected())
        {
            try {
                String msg = null;
                msg = in.readLine();
                if(log)
                    Log.i("GameMessManager", "received : " + msg);
                return msg;
            } catch (IOException e) {
                if(log)
                    Log.i("GameMessManager",
                            "attempted getNextMessage got exception : "
                                    + e.getMessage());
            }
        }
        else
        {
            if(log)
                Log.i("GameMessManager",
                        "attempted getNextMessage: not connected");
        }
        return null;
    }

    public static void setServerAddress(String addr)
    {
        servAddr = String.valueOf(addr);
    }

    public static void setPort(int port)
    {
        portConnect = port;
    }

    public static String getServerAddress() {
        return servAddr;
    }

    public static void logActivity(boolean state)
    {
        log = state;
    }
}