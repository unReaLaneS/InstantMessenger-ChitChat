package com.example.unreal.chitchat;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;

import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * A login screen that offers login via email/password.
 */
public class LoginActivity extends AppCompatActivity {

    public final static String USERNAME = "com.example.unreal.chitchat.USERNAME";
    public final static String PASSWORD = "com.example.unreal.chitchat.PASSWORD";

    // UI references.
    private AutoCompleteTextView mUsernameView;
    private EditText mPasswordView;
    private View mProgressView;
    private View mLoginFormView;
    private Client mTcpClient;
    private Intent intent;
    private String username;
    private String password;

    public Client getmTcpClient() {
        return mTcpClient;
    }

    public void setmTcpClient(Client mTcpClient) {
        this.mTcpClient = mTcpClient;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        // Set up the login form.

        mUsernameView = (AutoCompleteTextView) findViewById(R.id.username);
        //populateAutoComplete();

        mPasswordView = (EditText) findViewById(R.id.password);
        mPasswordView.setOnEditorActionListener(new TextView.OnEditorActionListener() {
            @Override
            public boolean onEditorAction(TextView textView, int id, KeyEvent keyEvent) {
                if (id == R.id.login || id == EditorInfo.IME_NULL) {
                    //attemptLogin();
                    return true;
                }
                return false;
            }
        });

        Button mSignInButton = (Button) findViewById(R.id.sign_in_button);

        mLoginFormView = findViewById(R.id.login_form);

        //mProgressView = findViewById(R.id.login_progress);
    }

    public void Login(View view) {
        mUsernameView.setError(null);
        mPasswordView.setError(null);
        if(mUsernameView.getText().toString().trim().equalsIgnoreCase(""))
        {
            mUsernameView.setError(getString(R.string.error_username_empty));
        }
        else if(mPasswordView.getText().toString().trim().equalsIgnoreCase(""))
        {
            mPasswordView.setError(getString(R.string.error_password_empty));
        }
        else {
            try {
                intent = new Intent(this, DisplayMainActivity.class);
                username = mUsernameView.getText().toString();
                intent.putExtra(USERNAME, username);
                password = mPasswordView.getText().toString();
                intent.putExtra(PASSWORD, password);
                ConnectTask connectTask = new ConnectTask(this);
                connectTask.execute("");
                mUsernameView.setText("");
                mPasswordView.setText("");
            }
            catch(Exception e)
            {
                Log.e("Activity","S:Error",e);
            }
        }
    }

    public boolean isInternetWorking() {
        boolean success = false;
        try {
            URL url = new URL("https://google.com");
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setConnectTimeout(10000);
            connection.connect();
            success = connection.getResponseCode() == 200;
        } catch (IOException e) {
            e.printStackTrace();
        }
        return success;
    }

    public class ConnectTask extends AsyncTask<String, String, Client> {


        ProgressDialog pdLoading = new ProgressDialog(LoginActivity.this);
        LoginActivity loginActivity;

        public ConnectTask(LoginActivity loginActivity) {
            this.loginActivity = loginActivity;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();

            //this method will be running on UI thread
            pdLoading.setMessage("\tConnecting...");
            pdLoading.show();
        }

        @Override
        protected Client doInBackground(String... message) {
            if(!isInternetWorking())
            {
                publishProgress("NoConnection");

                return null;
            }
            mTcpClient = new Client();
            mTcpClient.mLoginListener = new Client.LoginCheck(){
                //here the messageReceived method is implemented
                public void OnLoginOk() {

                    //this method calls the onProgressUpdate
                    publishProgress("OK");
                }
                public void OnLoginFailed() {
                    //this method calls the onProgressUpdate
                    publishProgress("Failed");
                }
            };

            mTcpClient.Login(username,password);

            return null;
        }

        /*@Override
        protected void onPostExecute(Client client) {
            super.onPostExecute(client);

            //this method will be running on UI thread

            pdLoading.dismiss();
        }*/

        @Override
        protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
                pdLoading.dismiss();
                if(values[0].equals("OK")) {
                    intent.putExtra("Friends",mTcpClient.getLoadfriends());
                    startActivity(intent);
                }
                else if(values[0].equals("Failed"))
                {
                    mUsernameView.setError(null);
                    mPasswordView.setError(null);
                    mUsernameView.setError(getString(R.string.error_invalid_username));
                    mPasswordView.setError(getString(R.string.error_incorrect_password));
                }
            else if(values[0].equals("NoConnection"))
                {
                    Context context = getApplicationContext();
                    CharSequence text = "No internet connection!";
                    int duration = Toast.LENGTH_LONG;

                    Toast toast = Toast.makeText(context, text, duration);
                    toast.show();
                }

        }
    }



}

