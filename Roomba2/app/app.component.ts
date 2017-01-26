import { Component,Input,NgZone } from '@angular/core';
import { CurrentStatus } from './CurrentStatus';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { RoombaService } from './roomba.service';
import './rxjs-operators';
import { OnInit } from '@angular/core';

@Component({
    selector: 'my-app',
    styles: [`
  .selected {
    background-color: #CFD8DC !important;
    color: white;
  }
  .missions {
    margin: 0 0 2em 0;
    list-style-type: none;
    padding: 0;
    width: 15em;
  }
  .missions th {
    cursor: pointer;
    position: relative;
    left: 0;
    background-color: #EEE;
    margin: .5em;
    padding: .3em 0;
    height: 1.6em;
    border-radius: 4px;
  }
  .missions li {
    cursor: pointer;
    position: relative;
    left: 0;
    background-color: #EEE;
    margin: .5em;
    padding: .3em 0;
    height: 1.6em;
    border-radius: 4px;
  }
  .missions li.selected:hover {
    background-color: #BBD8DC !important;
    color: white;
  }
  .missions li:hover {
    color: #607D8B;
    background-color: #DDD;
    left: .1em;
  }
  .missions .text {
    position: relative;
    top: -3px;
  }
  .missions .mission {
    display: inline-block;
    font-size: small;
    color: white;
    padding: 0.8em 0.7em 0 0.7em;
    background-color: #607D8B;
    line-height: 1em;
    position: relative;
    left: -1px;
    top: -4px;
    height: 1.8em;
    margin-right: .8em;
    border-radius: 4px 0 0 4px;
  }
`],
    templateUrl: 'app/app.component.html',
    providers: [RoombaService]
})
export class AppComponent implements OnInit {
    roombaStatus: CurrentStatus;
    roombaMissions: Mission[];
    selectedMission: Mission;
    errorMessage: string;
    name: string;

    constructor(private zone: NgZone,
                private roombaService: RoombaService) {
        this.roombaStatus = new CurrentStatus();
        this.roombaMissions = new Array<Mission>();
        this.errorMessage = "";
        this.name = 'Angular';
    }

    onSelect(mission: Mission)
    {
        this.selectedMission = mission;
    }
    updateStatus(status: CurrentStatus, error?: string) {
        if (status)
            this.roombaStatus = status;
        else if (error)
            this.errorMessage = error;

        //setTimeout(this.refreshCurrentStatusTimeout, 10000);
    }
    refreshCurrentStatusTimeout()
    {
        this.roombaService.getCurrentStatus()
            .subscribe(status => {
                this.zone.run(() => {
                    this.updateStatus(status);
                })
            },
            error => {
                this.zone.run(() => {
                    this.updateStatus(null,error);
                })
            });
    }

    ngOnInit()
    { 
        this.refreshCurrentStatusTimeout();

        this.roombaService.getMissions()
            .subscribe(missions => {
                this.zone.run(() => {
                    this.roombaMissions = missions;
                })
            },
            error => this.errorMessage = <any>error);
    }
}
