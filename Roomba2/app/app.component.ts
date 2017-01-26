import { Component,Input } from '@angular/core';
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
    roombaStatus_LastUpdate :string;
    roombaStatus_RoombaTime :string;
    roombaStatus_NextMission: string;
    nextMissionCountDown: Date;
    roombaMissions: Mission[];
    selectedMission: Mission;
    errorMessage: string;
    name: string;

    constructor(private roombaService: RoombaService) {
        this.roombaStatus = new CurrentStatus();
        this.roombaStatus_LastUpdate = "";
        this.roombaStatus_RoombaTime = "";
        this.roombaStatus_NextMission= "";
        this.roombaMissions = new Array<Mission>();
        this.errorMessage = "";
        this.name = 'Angular';
    }

    onSelect(mission: Mission)
    {
        this.selectedMission = mission;
    }

    locale(): string {
        //return (navigator.languages && navigator.languages.length) ? navigator.languages[0] : navigator.language;
        return navigator.language;
    }

    updateNextMissionCountdown() {
        if (this.nextMissionCountDown)
        {
            let days: number = this.nextMissionCountDown.getDay();
            let hours: number = this.nextMissionCountDown.getHours();
            let mins: number = this.nextMissionCountDown.getMinutes();
            let secs: number = this.nextMissionCountDown.getSeconds();

            if (!(days == 0 && hours == 0 && mins == 0 && secs == 0)) {
                secs--;
                if (secs < 0) {
                    secs = 0;
                    mins--;
                    if (mins < 0) {
                        mins = 0;
                        hours--;
                        if (hours < 0) {
                            hours = 0;
                            days--;
                        }
                    }
                }
            }
            this.nextMissionCountDown = new Date(0, 0, days, hours, mins, secs, 0);
            if (days == 0 && hours == 0 && mins == 0 && secs == 0) {
                this.roombaStatus_NextMission = 'Now!'
            }
            else {
                let s: string = "";
                if (days > 0)
                    s += days.toString() + "d ";
                if (!(days == 0 && hours == 0))
                    s += hours.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "h ";
                if (!(days == 0 && hours == 0 && mins == 0))
                    s += mins.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "m ";
                s += secs.toLocaleString(this.locale(), { minimumIntegerDigits: 2 }) + "s";

                this.roombaStatus_NextMission = s;
            }
        }
        setTimeout(() => { this.updateNextMissionCountdown(); }, 1000);
    }

    updateStatus(status: CurrentStatus, error?: string) {
        if (status) {
            this.roombaStatus = status;

            this.nextMissionCountDown = new Date(status.NextMission);

            // Date pipes not working on IE
            this.roombaStatus_LastUpdate = new Date(status.LastUpdate).toLocaleDateString(this.locale(), {
                day: 'numeric',
                month: 'short',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            });
            this.roombaStatus_RoombaTime = new Date(status.RoombaTime).toLocaleTimeString(this.locale(), {
                hour: '2-digit',
                minute: '2-digit'
            });
        }
        else if (error)
            this.errorMessage = error;

        setTimeout(() => { this.refreshCurrentStatusTimeout(); }, 10000);
    }
    refreshCurrentStatusTimeout()
    {
        this.roombaService.getCurrentStatus()
            .subscribe(status => {
                    this.updateStatus(status);
            },
            error => {
                    this.updateStatus(null,error);
            });
    }

    ngOnInit()
    { 
        this.refreshCurrentStatusTimeout();
        this.updateNextMissionCountdown();

        this.roombaService.getMissions()
            .subscribe(missions => {
                    this.roombaMissions = missions;
            },
            error => this.errorMessage = <any>error);
    }
}
