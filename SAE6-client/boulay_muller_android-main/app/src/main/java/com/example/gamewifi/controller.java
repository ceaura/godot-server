package com.example.gamewifi;

import static com.example.gamewifi.GameMessageManager.getNextMessage;
import static com.example.gamewifi.GameMessageManager.isConnected;
import static com.example.gamewifi.GameMessageManager.sendMessage;

import static java.lang.Thread.sleep;

import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.Switch;

import androidx.appcompat.app.AppCompatActivity;

import android.view.View;

public class controller extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.remote_controller);
        FrameLayout joystick = (FrameLayout) findViewById(R.id.joystick);
        FrameLayout joystick2 = (FrameLayout) findViewById(R.id.joystick2);


        new Thread(new Runnable() {
            public void run() {
                /*while (true) {
                    //sendMessage("LIVE");
                    Log.i("antiAFK", "message envoyer au server");
                    try {
                        sleep(9000);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }*/
            }
        }).start();

        joystick.addView(new JoystickView(this, new JoystickView.ValueChangedHandler() {
            @Override
            public void onValueChanged(int Vg, int Vd) {
                new Thread(new Runnable() {
                    public void run() {
                if (isConnected()) {
                    //sendMessage("ORIENT");
                    //String orient = getNextMessage();
                    //Log.i("robot", "l'orientation est de "+orient);
                    sendMessage("MotL=" + ((float) (Vg) / 200 + 0.5) + "#MotR=" + ((float) (Vd) / 200 + 0.5));
                    Log.i("robot", "MotL=" + ((float) (Vg) / 200 + 0.5) + "#MotR=" + ((float) (Vd) / 200 + 0.5));
                    try {
                        sleep(100);
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }
                else{
                    Log.i("debug", "onCreate: n'est pas connecter");
                }
                    }
                }).start();
            }
        }));


        joystick2.addView(new JoystickView(this, new JoystickView.ValueChangedHandler() {
            @Override
            public void onValueChanged(int Vg, int Vd) {
                new Thread(new Runnable() {
                    public void run() {
                        if (isConnected()) {
                            //sendMessage("ORIENT");
                            //String orient = getNextMessage();
                            //Log.i("robot", "l'orientation est de "+orient);

                            if (Vg<100){
                                sendMessage("GunTrav=" + ((Vg / 400)));
                                Log.i("robot", "angle tir 1=" + ((float)(Vg )));
                                }
                            }
                            if (Vd>0) {
                                sendMessage("GunTrav=" + ((Vd / 400)));
                                Log.i("robot", "angle tir 2=" + ((Vd/ 400)));
                            try {
                                sleep(100);
                            } catch (InterruptedException e) {
                                e.printStackTrace();
                            }
                        }
                        else{
                            Log.i("debug", "onCreate: n'est pas connecter");
                        }
                    }
                }).start();
            }
        }));



    }

    public void shooter(View view){
        new Thread(new Runnable() {
            public void run() {
                if (isConnected()) {
                    Switch simpleSwitch = (Switch) findViewById(R.id.switch1);
                    if (simpleSwitch.isChecked()) {
                        sendMessage("GunTrig=1");
                        Log.i("debug", "tire");
                    }
                    else{
                        sendMessage("GunTrig=0");
                        Log.i("debug", "tire pas");
                    }
                }
                else{
                    Log.i("debug", "onCreate: n'est pas connecter");
                }
            }
        }).start();
    }

    public void bot(View view){
        new Thread(new Runnable() {
            public void run() {
                while (true) {
                    Switch simpleSwitch = (Switch) findViewById(R.id.switch2);
                    if (simpleSwitch.isChecked()) {
                        sendMessage("CBOT");
                        String pos = getNextMessage();
                        Log.i("bot", "run: "+pos);;
                        Float orient =  Float.parseFloat(pos.split("/")[0]);
                        Float posistion =  Float.parseFloat(pos.split("/")[1]);
                        if (orient>0.5){
                            try{
                            sendMessage("MotR=1#GunTrav="+orient);
                            }catch (Exception e){
                                Log.i("bot","server is unconnect");
                            }
                            if(posistion>10) {
                                try{
                                sendMessage("MotL=0.75#GunTrav="+orient);
                                }catch (Exception e){
                                    Log.i("bot","server is unconnect");
                                }
                            }
                            Log.i("bot", "position" + orient + "moteur droite");

                        }
                        else {
                            if(posistion>10) {
                                try{
                                sendMessage("MotR=0.75#GunTrav="+orient);
                                }catch (Exception e){
                                    Log.i("bot","server is unconnect");
                                }
                            }
                            try{
                            sendMessage("MotL=1#GunTrav="+orient);
                            }catch (Exception e){
                                Log.i("bot","server is unconnect");
                            }
                            Log.i("bot", "position" + orient+"moteur gauche");
                            try {
                                sleep(10);
                            } catch (InterruptedException e) {
                                e.printStackTrace();
                            }
                        }
                    }else {
                        try {
                            sleep(1000);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                }
            }
        }).start();
    }

    public void msg(View view){
        final String[] msg = {""};
        int id = view.getId();
        switch (id){
            case R.id.msgLbtn:
                Log.i("ID btn ", String.valueOf(R.id.msgLbtn )+ "-->" +  id);
                Button button = (Button)findViewById(R.id.msgLbtn);
                msg[0] = button.getText().toString();
                break;

            case R.id.msgLbtn2:
                Log.i("ID btn ", String.valueOf(R.id.msgLbtn2) + "-->" + id);
                button = (Button)findViewById(R.id.msgLbtn2);
                msg[0] = button.getText().toString();
                break;

            case R.id.addMsgPerso:
                Log.i("ID btn ", String.valueOf(R.id.addMsgPerso) + "-->" +  id);
                EditText text = (EditText) findViewById(R.id.msgPerso);
                msg[0] = text.getText().toString();
                break;
        }
        new Thread(new Runnable() {
            public void run() {
                if (isConnected()) {
                    Log.i("ID", "l'ID du btn est "+id);
                    sendMessage("MSG="+ msg[0]);
                    Log.i("MSG", "msg good --> " + msg[0]);
                }
                else{
                    Log.i("debug", "onCreate: msg not good");
                }
            }
        }).start();
    }



}
