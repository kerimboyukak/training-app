// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace Identity.UI.Pages.Logout;

public static class LogoutOptions
{
    public static readonly bool ShowLogoutPrompt = true;
    public static readonly bool AutomaticRedirectAfterSignOut = true; // Making sure that the user is redirected to the PostLogoutRedirectUris url of the client
}
