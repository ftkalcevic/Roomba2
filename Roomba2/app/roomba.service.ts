import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { CurrentStatus } from './CurrentStatus';
import { Mission } from './Mission';
import { MissionDetails } from './MissionDetails';
import { LiveMissionDetails } from './LiveMissionDetails';


@Injectable()
export class RoombaService {
    constructor(private http: Http) { }

    getCurrentStatus(): Observable<CurrentStatus> {
        return this.http.get("api/CurrentStatus")
            .map(this.extractData)
            .catch(this.handleError);
    }

    getMissions(): Observable<Mission[]> {
        return this.http.get("api/Missions")
            .map(this.extractData)
            .catch(this.handleError);
    }

    getMissionDetails(id:number): Observable<MissionDetails> {
        return this.http.get("api/MissionDetails/"+id.toString())
            .map(this.extractData)
            .catch(this.handleError);
    }

    getLiveMissionDetails(lastTick: number): Observable<LiveMissionDetails> {
        return this.http.get("api/LiveMissionDetails/" + lastTick.toString())
            .map(this.extractData)
            .catch(this.handleError);
    }

    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }
    private handleError(error: Response | any) {
        // In a real world app, we might use a remote logging infrastructure
        let errMsg: string;
        if (error instanceof Response) {
            const body = error.json() || '';
            const err = body.error || JSON.stringify(body);
            errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
        } else {
            errMsg = error.message ? error.message : error.toString();
        }
        console.error(errMsg);
        return Observable.throw(errMsg);
    }
}
