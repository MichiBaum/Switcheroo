using ManagedWinapi.Accessibility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Windows.Contents
{
    internal class ListViewParser : WindowContentParser {
        internal override bool CanParseContent(SystemWindow sw) {
            const uint LVM_GETITEMCOUNT = 0x1000 + 4;
            int cnt = sw.SendGetMessage(LVM_GETITEMCOUNT);
            return cnt != 0;
        }

        internal override WindowContent ParseContent(SystemWindow sw) {
            const uint LVM_GETITEMCOUNT = 0x1000 + 4;
            int cnt = sw.SendGetMessage(LVM_GETITEMCOUNT);
            if (cnt == 0)
                throw new Exception();
            SystemAccessibleObject o = SystemAccessibleObject.FromWindow(sw, AccessibleObjectID.OBJID_CLIENT);
            if (o.RoleIndex == 33) {
                // are there column headers?
                int cs = o.Children.Length;
                string[] hdr = null;
                if (cs > 0) {
                    SystemAccessibleObject headers = o.Children[cs - 1];
                    if (headers.RoleIndex == 9 && headers.Window != sw) {
                        SystemAccessibleObject hdrL =
                            SystemAccessibleObject.FromWindow(headers.Window, AccessibleObjectID.OBJID_CLIENT);
                        hdr = new string[hdrL.Children.Length];
                        for (int i = 0; i < hdr.Length; i++) {
                            if (hdrL.Children[i].RoleIndex != 25) {
                                hdr = null;
                                break;
                            }

                            hdr[i] = hdrL.Children[i].Name;
                        }

                        if (hdr != null) cs--;
                    }
                }

                List<string> values = new();
                for (int i = 0; i < cs; i++)
                    if (o.Children[i].RoleIndex == 34) {
                        string name = o.Children[i].Name;
                        if (hdr != null)
                            try {
                                string cols = o.Children[i].Description;
                                if (cols == null && values.Count == 0) { hdr = null; } else {
                                    string tmpCols = "; " + cols;
                                    List<string> usedHdr = new();
                                    foreach (string header in hdr) {
                                        string h = "; " + header + ": ";
                                        if (tmpCols.Contains(h)) {
                                            usedHdr.Add(header);
                                            tmpCols = tmpCols.Substring(tmpCols.IndexOf(h) + h.Length);
                                        }
                                    }

                                    foreach (string header in hdr) {
                                        name += "\t";
                                        if (usedHdr.Count > 0 && usedHdr[0] == header) {
                                            if (!cols.StartsWith(header + ": "))
                                                throw new Exception();
                                            cols = cols.Substring(header.Length + 1);
                                            string elem;
                                            if (usedHdr.Count > 1) {
                                                int pos = cols.IndexOf("; " + usedHdr[1] + ": ");
                                                elem = cols.Substring(0, pos);
                                                cols = cols.Substring(pos + 2);
                                            } else {
                                                elem = cols;
                                                cols = "";
                                            }

                                            name += elem;
                                            usedHdr.RemoveAt(0);
                                        }
                                    }
                                }
                            } catch (COMException ex) {
                                if (ex.ErrorCode == -2147352573 && values.Count == 0)
                                    hdr = null;
                                else
                                    throw ex;
                            }

                        values.Add(name);
                    }

                if (hdr != null) {
                    string lines = "", headers = "";
                    foreach (string h in hdr) {
                        if (lines.Length > 0)
                            lines += "\t";
                        if (headers.Length > 0)
                            headers += "\t";
                        headers += h;
                        lines += ListContent.Repeat('~', h.Length);
                    }

                    values.Insert(0, lines);
                    values.Insert(0, headers);
                    return new ListContent("DetailsListView", -1, null, values.ToArray());
                }

                return new ListContent("ListView", -1, null, values.ToArray());
            }

            return new ListContent("EmptyListView", -1, null, new string[0]);
        }
    }
}