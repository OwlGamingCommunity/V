#define DEV_ENVIRONMENT


public static class PlayfabWebAPI
{
#if DEV_ENVIRONMENT
	public static string g_TitleID = "F27A";
#elif STAFF_BETA_ENVIRONMENT
	public static string g_TitleID = ""; // TODO_LAUNCH: Fill this in
#elif BETA_ENVIRONMENT
	public static string g_TitleID = ""; // TODO_LAUNCH: Fill this in
#elif PROD_ENVIRONMENT
	public static string g_TitleID = ""; // TODO_LAUNCH: Fill this in
#endif
}
