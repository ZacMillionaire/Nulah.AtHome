export interface EventRequestFormModel {
	Description: string;
	Id: number;
	Tags: string | null;
	Start: string;
	End: string | null;
	Version : number;
}