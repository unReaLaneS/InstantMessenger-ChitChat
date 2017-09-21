package com.example.unreal.chitchat;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import java.util.ArrayList;

public class DisplayMainActivity extends AppCompatActivity {

    private ArrayList<String> arrayList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_display_main);


        Intent intent = getIntent();
        String username = intent.getStringExtra(LoginActivity.USERNAME);
        String password = intent.getStringExtra(LoginActivity.PASSWORD);

        arrayList = intent.getStringArrayListExtra("Friends");
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1,arrayList);
        ListView listView = (ListView) findViewById(R.id.friend_list);
        listView.setAdapter(adapter);


        adapter.notifyDataSetChanged();

    }

    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main, menu);
        // Action View
        //MenuItem searchItem = menu.findItem(R.id.action_search);
        //SearchView searchView = (SearchView) MenuItemCompat.getActionView(searchItem);
        // Configure the search info and add any event listeners
        //return super.onCreateOptionsMenu(menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        //handle presses on the action bar items
        switch (item.getItemId()) {

            case R.id.action_add_contact:
                //startActivity(new Intent(this, AddContact.class));
                return true;

            case R.id.action_about:
                //startActivity(new Intent(this, AboutContacts.class));
                return true;

            case R.id.action_settings:
                //startActivity(new Intent(this, Settings.class));
                return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
