import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private http:HttpClient)
  {
    //constructor
  }

  ngOnInit() {
    this.httpGet()
  }
  title = 'The Dating App!';
  name="daniyal";
  id :any ;
  Users:any;

  httpGet()
  {
    this.http.get("https://localhost:5001/api/users").subscribe(res =>{
      console.log("get the response");
      this.Users = res;
      console.log(res);
      
    },err=>{
      console.log(err);
    });
    
  }
}
