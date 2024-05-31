package com.example.gamewifi;

import static java.lang.Thread.sleep;

import static com.example.gamewifi.GameMessageManager.isConnected;
import static com.example.gamewifi.GameMessageManager.connect;


import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.skydoves.colorpickerview.ColorEnvelope;
import com.skydoves.colorpickerview.ColorPickerView;
import com.skydoves.colorpickerview.listeners.ColorEnvelopeListener;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;


public class MainActivity extends AppCompatActivity {

    static int[] colorCar;
    private String color;

    private EditText mLoginEditText;
    private Spinner mIpSpinner;

    private Spinner mLogSpinner;

    private ArrayAdapter<String> mAdapter;
    private ArrayAdapter<String> mAdapterLog;

    private List<String> mIpList;

    private List<String> mLogList;
    private static final String IP_FILENAME = "ips.txt";
    private static final String LOGIN_FILENAME = "login.txt";


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ColorPickerView colorPickerView = findViewById(R.id.colorPickerView);

//        clearIpsFile();


        colorPickerView.setColorListener(new ColorEnvelopeListener() {
            @Override
            public void onColorSelected(ColorEnvelope envelope, boolean fromUser) {
                LinearLayout linearLayout = findViewById(R.id.colorBack);
                linearLayout.setBackgroundColor(envelope.getColor());
                colorCar = envelope.getArgb();

                color = colorCar[1] +"="+colorCar[2] +"="+colorCar[3];
                Log.i("mondebug", "Couleur choisit = " + color + " lenght=" + colorCar.length);
            }
        });

// Initialiser le Spinner et l'adaptateur
        mLoginEditText = findViewById(R.id.log);
        mIpSpinner = findViewById(R.id.ip_spinner);

        mAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, new ArrayList<>());
        mAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        mIpSpinner.setAdapter(mAdapter);
        mIpList = new ArrayList<>();

        mLogSpinner = findViewById(R.id.log_spinner);
        mAdapterLog = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, new ArrayList<>());
        mAdapterLog.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        mLogSpinner.setAdapter(mAdapterLog);
        mLogList = new ArrayList<>();


// Charger les adresses IP à partir du fichier et les ajouter à l'adaptateur
        loadIPsFromFile();
        loadLoginFromFile();

    }

    @Override
    protected void onResume(){
        super.onResume();
        new Thread(new Runnable() {
            public void run() {
                if (isConnected()) {
                    GameMessageManager.sendMessage("EXIT");
                    Log.i("conexion", "deconnexion");
                }
            }
        }).start();
        // Recharger les adresses IP à partir du fichier
        mAdapter.clear();
        mAdapterLog.clear();
        loadIPsFromFile();
        loadLoginFromFile();
    }




    public void connect(View view){

        // Enregistrer l'adresse IP dans le stockage interne lorsque l'utilisateur clique sur un bouton
        String ip = ((EditText) findViewById(R.id.ip_server)).getText().toString();
        String login = mLoginEditText.getText().toString();
        Log.i("Ip serveur", "ip =" + ip);
        GameMessageManager.connect(ip);
        if (isConnected()){
            Log.i("Connected", "connect to" + ip);
        }

        mIpList.add(ip);
        mLogList.add(login);

        // Vider le fichier et réécrire toutes les adresses IP
        if (!mIpList.contains(ip)) {
            try {
                FileOutputStream fileOutputStream = openFileOutput(IP_FILENAME, MODE_PRIVATE);
                fileOutputStream.write((ip + "\n").getBytes());
                fileOutputStream.close();
                Toast.makeText(MainActivity.this, "Adresse IP enregistrée", Toast.LENGTH_SHORT).show();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

        // Enregistrer le login dans le fichier
        if (!mLogList.contains(login)) {
            try {
                FileOutputStream fileOutputStream = openFileOutput(LOGIN_FILENAME, MODE_PRIVATE);
                fileOutputStream.write((login + "\n").getBytes());
                fileOutputStream.close();
                Toast.makeText(MainActivity.this, "Login enregistré", Toast.LENGTH_SHORT).show();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }


        new Thread(new Runnable() {
            public void run() {
                try {
                    sleep(50);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
                if (isConnected()){
                    Spinner mLogSpinner2 = (Spinner) findViewById(R.id.log_spinner);
                    String logSpinner = mLogSpinner2.getSelectedItem().toString();

                    if (!login.isEmpty()) {
                        // Si le champ de saisie 'log' a été rempli, ajouter sa valeur à la requête
                        GameMessageManager.sendMessage("NAME="+login+"#COL="+color);
                    } else if (!logSpinner.isEmpty()) {
                        // Si une valeur a été sélectionnée dans le spinner 'log_spinner', ajouter sa valeur à la requête
                        GameMessageManager.sendMessage("NAME="+logSpinner+"#COL="+color);
                    } else {
                        // Si aucun champ n'a été rempli, afficher un message d'erreur
                        Toast.makeText(MainActivity.this, "Veuillez remplir le champ 'log' ou sélectionner une valeur dans le spinner", Toast.LENGTH_SHORT).show();
                    }
                    Log.i("Nom et couleur", "Nom ="+login + "couleur = "+color);
                }
            }
        }).start();

        Log.i("mondebug", "etape1 ");

        Intent intent = new Intent(this, controller.class);
        startActivity(intent);
    }

    private void loadIPsFromFile() {
        try {
            FileInputStream fileInputStream = openFileInput(IP_FILENAME);
            InputStreamReader inputStreamReader = new InputStreamReader(fileInputStream);
            BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
            String line;
            while ((line = bufferedReader.readLine()) != null) {
                mIpList.add(line);
            }
            bufferedReader.close();
            inputStreamReader.close();
            fileInputStream.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        mAdapter.addAll(mIpList);
        mAdapter.notifyDataSetChanged();
    }

    private void loadLoginFromFile() {
        try {
            FileInputStream fileInputStream = openFileInput(LOGIN_FILENAME);
            InputStreamReader inputStreamReader = new InputStreamReader(fileInputStream);
            BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
            String line2;
            while ((line2 = bufferedReader.readLine()) != null) {
                mLogList.add(line2);
            }
            bufferedReader.close();
            inputStreamReader.close();
            fileInputStream.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        mAdapterLog.addAll(mLogList);
        mAdapterLog.notifyDataSetChanged();
    }

//    public void clearIpsFile() {
//        try {
//            PrintWriter writer = new PrintWriter(new FileWriter(getFilesDir() + "/ips.txt"));
//            writer.print("");
//            writer.close();
//        } catch (IOException e) {
//            e.printStackTrace();
//        }
//    }


}