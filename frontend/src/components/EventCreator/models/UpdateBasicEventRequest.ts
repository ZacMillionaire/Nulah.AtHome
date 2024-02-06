export interface UpdateBasicEventRequest {
	Description: string;
	Id: number;
	Tags: string[] | null;
	Start: Date;
	End: Date | null;
	Version : number;
}