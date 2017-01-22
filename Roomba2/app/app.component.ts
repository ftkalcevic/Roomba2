import { Component,Input,NgZone } from '@angular/core';
import { CurrentStatus } from './CurrentStatus';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { RoombaService } from './roomba.service';
import './rxjs-operators';
import { OnInit } from '@angular/core';

@Component({
  selector: 'my-app',
  template: `
<h1>Roomba Bob</h1>
<table>
<tr><th>Last Update</th><th>Status</th><th>Next Mission</th><th>Roomba Time</th></tr>
<tr><td>{{roombaStatus.LastUpdate}}</td><td>{{roombaStatus.Status}}</td><td>{{roombaStatus.NextMission}}</td><td>{{roombaStatus.RoombaTime}}</td></tr>
</table>

<span>Number of Missions: {{roombaMissions.length}}</span>
<li *ngFor="let mission of roombaMissions">
  <span>{{mission.MissionId}} {{mission.StartTime}}</span>
</li>
<span>MissionId: {{details.MissionId}} Start: {{details.StartTime}} End: {{details.EndTime}} </span>
<li *ngFor="let x of details.x; let i = index">
  <span>{{details.x[i]}},{{details.y[i]}}:{{details.theta[i]}} {{details.battery[i]}}</span>
</li>

<span>{{errorMessage}}</span>
`,
  providers: [RoombaService]
})
export class AppComponent implements OnInit {
    roombaStatus: CurrentStatus;
    roombaMissions: Mission[];
    details: MissionDetails;
    errorMessage: string;
    name: string;

    constructor(private zone: NgZone,
                private roombaService: RoombaService) {
        this.roombaStatus = new CurrentStatus();
        this.roombaMissions = new Array<Mission>();
        this.details = new MissionDetails();
        this.errorMessage = "";
        this.name = 'Angular';
    }

    UpdateMissions(missions:Mission[])
    {
        this.roombaMissions = missions;

        this.roombaService.getMissionDetails(missions[0].MissionId)
            .subscribe(mission => {
                this.zone.run(() => {
                    this.details = mission;
                })
            },
            error => this.errorMessage = <any>error);
    }
    ngOnInit()
    { 
        this.roombaService.getCurrentStatus()
            .subscribe(status => {
                this.zone.run(() => {
                    this.roombaStatus = status;
                })
            },
            error => this.errorMessage = <any>error);
        //this.roombaService.getMissions()
        //    .subscribe(missions => {
        //        this.zone.run(() => {
        //            this.roombaMissons = missions;
        //        })
        //    },
        //    error => this.errorMessage = <any>error);
        this.roombaService.getMissions()
            .subscribe(missions => {
                this.zone.run(() => {
                    this.UpdateMissions(missions);
                })
            },
            error => this.errorMessage = <any>error);

    }
}
